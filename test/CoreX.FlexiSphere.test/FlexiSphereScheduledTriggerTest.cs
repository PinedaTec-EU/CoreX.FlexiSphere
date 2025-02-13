#region Header

// --------------------------------------------------------------------------------------
// Powered by:
// 
//     __________.__                  .___    ___________                             
//     \______   \__| ____   ____   __| _/____\__    ___/___   ____       ____  __ __ 
//      |     ___/  |/    \_/ __ \ / __ |\__  \ |    |_/ __ \_/ ___\    _/ __ \|  |  \
//      |    |   |  |   |  \  ___// /_/ | / __ \|    |\  ___/\  \___    \  ___/|  |  /
//      |____|   |__|___|  /\___  >____ |(____  /____| \___  >\___  > /\ \___  >____/ 
//                   \/     \/     \/     \/           \/     \/  \/     \/
// 
// 
// FileName: FlexiSphereScheduledTriggerTest.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.099Z
//
// --------------------------------------------------------------------------------------

#endregion

using CoreX.extensions;
using CoreX.FlexiSphere.triggers;

using Shouldly;

namespace CoreX.FlexiSphere.test;

public class FlexiSphereScheduledTriggerTest : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;

    public FlexiSphereScheduledTriggerTest(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact]
    public void ConfigureTrigger_WhenCronExpressionIsNullOrEmpty_ShouldUseDefaultCronTime()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        var cronExpression = string.Empty;

        // Act
        trigger.ConfigureTrigger(cronExpression);

        // Assert
        trigger.CronTime.ShouldBe("0,5,10,15,20,25,30,35,40,45,50,55 * * * *");
    }

    [Fact]
    public void ConfigureTrigger_ShouldUseCronExpression()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        var cronExpression = "0 0/20 * * * *";

        // Act
        trigger.ConfigureTrigger(cronExpression, 5, 7);

        // Assert
        trigger.CronTime.ShouldBe("0,20,40 * * * *");
        trigger.MaxConcurrents.ShouldBe(5);
        trigger.MaxOccurrences.ShouldBe(7);
    }

    [Fact]
    public async Task ActivateTrigger()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *"); // Every second

        // Act
        trigger.MaxConcurrents = 1;
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        int triggerCounter = 0;
        trigger.OnTriggered += (sender, args) => triggerCounter++;

        await AppsHelper.DelayWhileAsync(1000, () => triggerCounter == 0, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBe(triggerCounter);
        triggerCounter.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task ActivateTrigger_FireOnStart()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *"); // Every second

        // Act
        trigger.FireTriggerOnStart = true;
        trigger.MaxConcurrents = 1;
        trigger.MaxOccurrences = 2;
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        bool isTriggered = false;
        trigger.OnTriggered += (sender, args) => isTriggered = true;

        await AppsHelper.DelayWhileAsync(5000, () => !isTriggered, cancellationToken: TestContext.Current.CancellationToken);

        var nextExecution = trigger.GetNextOccurrence();

        // Assert
        trigger.Counter.ShouldBe(2);
        isTriggered.ShouldBeTrue();

        nextExecution.ShouldNotBeNull();
        nextExecution.Value.ShouldBeLessThan(DateTime.Now);
    }

    [Fact]
    public async Task DeactivateTrigger()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0 0 0 * * *"); // Every day at 00:00h

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        bool isTriggered = false;
        trigger.OnTriggered += (sender, args) => isTriggered = true;
        bool isCompleted = false;
        trigger.OnCompleted += (sender, args) => isCompleted = true;

        trigger.DeactivateTrigger("Test");

        await AppsHelper.DelayWhileAsync(5000, () => !isCompleted, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBe(0);
        isTriggered.ShouldBeFalse();
        isCompleted.ShouldBeTrue();
    }

    [Fact]
    public async Task ActivateTrigger_LimitConcurrences()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *", 2); // Every second
        trigger.MaxConcurrents.ShouldBe(2);

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        int isTriggered = 0;
        trigger.OnTriggered += (sender, args) =>
        {
            isTriggered++;
            Task.Delay(10000, TestContext.Current.CancellationToken).Wait(TestContext.Current.CancellationToken);
        };

        bool isCompleted = false;
        trigger.OnCompleted += (sender, args) => isCompleted = true;
        bool isFaulted = false;
        trigger.OnFaulted += (sender, args, exception) => isFaulted = true;

        await AppsHelper.DelayWhileAsync(5000, () => isTriggered < 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isFaulted.ShouldBeFalse();
        trigger.Counter.ShouldBeGreaterThanOrEqualTo(2);
        isTriggered.ShouldBeGreaterThanOrEqualTo(2);
        isCompleted.ShouldBeFalse();
    }

    [Fact]
    public async Task ActivateTrigger_LimitOccurrences()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *"); // Every second
        trigger.MaxOccurrences = 2;
        // trigger.MaxConcurrent = 0;

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        int isTriggered = 0;
        trigger.OnTriggered += async (sender, args) =>
        {
            isTriggered++;
            await Task.Delay(10000, TestContext.Current.CancellationToken);
        };

        bool isCompleted = false;
        trigger.OnCompleted += (sender, args) => isCompleted = true;

        await AppsHelper.DelayWhileAsync(5000, () => !isCompleted && isTriggered < 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBe(2);
        isTriggered.ShouldBe(2);
    }

    [Fact]
    public async Task ActivateTrigger_Cancel()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *"); // Every second

        using CancellationTokenSource cts = new();

        // Act
        trigger.ActivateTrigger(null, cts.Token);

        bool isCompleted = false;
        trigger.OnCompleted += (sender, args) => isCompleted = true;

        bool isCanceled = false;
        trigger.OnCanceled += (sender, args) => isCanceled = true;

        bool isTriggered = false;
        trigger.OnTriggered += (sender, args) =>
        {
            isTriggered = true;
            cts.Cancel();
        };

        await AppsHelper.DelayWhileAsync(5000, () => !isCanceled, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isCanceled.ShouldBeTrue();
        isCompleted.ShouldBeTrue();
        isTriggered.ShouldBeTrue();
    }

    [Fact]
    public async Task ActivateTrigger_Faulted_OnTriggered()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *"); // Every second
        trigger.MaxOccurrences = 2;

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        bool isFaulted = false;
        trigger.OnFaulted += (sender, args, exception) => isFaulted = true;

        bool isTriggered = false;
        trigger.OnTriggered += (sender, args) =>
        {
            isTriggered = true;
            throw new Exception("Test");
        };

        await AppsHelper.DelayWhileAsync(5000, () => !isFaulted, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isFaulted.ShouldBeTrue();
        isTriggered.ShouldBeTrue();
        trigger.Counter.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ActivateTrigger_Faulted_OnCompleted()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *", 10, 1); // Every second
        trigger.MaxOccurrences.ShouldBe(1);

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        bool isFaulted = false;
        trigger.OnFaulted += (sender, args, exception) => isFaulted = true;

        bool isCompleted = false;
        trigger.OnCompleted += (sender, args) =>
        {
            isCompleted = true;
            throw new Exception("Test");
        };

        await AppsHelper.DelayWhileAsync(5000, () => !isFaulted, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isCompleted.ShouldBeTrue();
        isFaulted.ShouldBeTrue();
    }

    [Fact]
    public void ConfigureTrigger_InvalidCronExpression()
    {
        // Arrange
        IFlexiSphereScheduledTrigger trigger = new FlexiSphereScheduledTrigger();
        var cronExpression = "abcdefg";

        // Act
        var action = () => trigger.ConfigureTrigger(cronExpression);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }
}
