using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexiSphere.triggers;
using Shouldly;

namespace FlexiSphere.test;

public class FlexiSphereScheduledTriggerTest
{
    [Fact]
    public void ConfigureTrigger_WhenCronExpressionIsNullOrEmpty_ShouldUseDefaultCronTime()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
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
        var trigger = new FlexiSphereScheduledTrigger();
        var cronExpression = "0 0/20 * * * *";

        // Act
        trigger.ConfigureTrigger(cronExpression, 5, 7);

        // Assert
        trigger.CronTime.ShouldBe("0,20,40 * * * *");
        trigger.MaxConcurrent.ShouldBe(5);
        trigger.MaxOccurrences.ShouldBe(7);
    }

    [Fact]
    public async Task ActivateTrigger()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *"); // Every second

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        bool isTriggered = false;
        trigger.OnTriggered += (sender, args) => isTriggered = true;

        await TestHelper.DelayWhileAsync(5000, () => !isTriggered, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBeGreaterThanOrEqualTo(1);
        isTriggered.ShouldBeTrue();
    }

    [Fact]
    public async Task DeactivateTrigger()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0 0 0 * * *"); // Every day at 00:00h

        // Act
        trigger.ActivateTrigger(cancellationToken: TestContext.Current.CancellationToken);

        bool isTriggered = false;
        trigger.OnTriggered += (sender, args) => isTriggered = true;
        bool isCompleted = false;
        trigger.OnCompleted += (sender, args) => isCompleted = true;

        trigger.DeactivateTrigger("Test");

        await TestHelper.DelayWhileAsync(5000, () => !isCompleted, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBe(0);
        isTriggered.ShouldBeFalse();
        isCompleted.ShouldBeTrue();
    }

    [Fact]
    public async Task ActivateTrigger_LimitConcurrences()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *", 2); // Every second
        trigger.MaxConcurrent.ShouldBe(2);

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

        await TestHelper.DelayWhileAsync(5000, () => isTriggered < 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBeGreaterThanOrEqualTo(2);
        isTriggered.ShouldBeGreaterThanOrEqualTo(2);
        isCompleted.ShouldBeFalse();
        isFaulted.ShouldBeFalse();
    }

    [Fact]
    public async Task ActivateTrigger_LimitOccurrences()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
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

        await TestHelper.DelayWhileAsync(5000, () => !isCompleted && isTriggered < 2, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        trigger.Counter.ShouldBe(2);
        isTriggered.ShouldBe(2);
    }

    [Fact]
    public async Task ActivateTrigger_Cancel()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
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

        await TestHelper.DelayWhileAsync(5000, () => !isCanceled, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isCanceled.ShouldBeTrue();
        isCompleted.ShouldBeTrue();
        isTriggered.ShouldBeTrue();
    }

    [Fact]
    public async Task ActivateTrigger_Faulted_OnTriggered()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
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

        await TestHelper.DelayWhileAsync(5000, () => !isFaulted, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isFaulted.ShouldBeTrue();
        isTriggered.ShouldBeTrue();
        trigger.Counter.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ActivateTrigger_Faulted_OnCompleted()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger("0/1 * * * * *", 10, 2); // Every second
        trigger.MaxOccurrences.ShouldBe(2);

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

        await TestHelper.DelayWhileAsync(5000, () => !isFaulted, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        isFaulted.ShouldBeTrue();
        isCompleted.ShouldBeTrue();
    }

    [Fact]
    public void ConfigureTrigger_InvalidCronExpression()
    {
        // Arrange
        var trigger = new FlexiSphereScheduledTrigger();
        var cronExpression = "abcdefg";

        // Act
        var action = () => trigger.ConfigureTrigger(cronExpression);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }
}
