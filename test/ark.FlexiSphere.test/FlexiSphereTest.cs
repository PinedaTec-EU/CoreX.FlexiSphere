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
// FileName: FlexiSphereTest.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.100Z
//
// --------------------------------------------------------------------------------------

#endregion

using Moq;

using Shouldly;

namespace ark.FlexiSphere.test;

public class FlexiSphereTest
{
    [Fact]
    public async Task Start_WithOut_Settings()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();
        // Act
        var action = async () => await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await action.ShouldThrowAsync<FlexiSphereException>();
    }

    [Fact]
    public void AddJob_Null()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var action = () => tsphere.AddJob(null!);
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void AddJobs()
    {
        bool subscribed1 = false;
        var job1 = new Mock<IFlexiSphereJob>();
        job1.SetupAdd(j => j.OnFaulted += It.IsAny<FlexiSphereJobExceptionHandler<Exception>>())
            .Callback(() => subscribed1 = true);
        bool subscribed2 = false;
        var job2 = new Mock<IFlexiSphereJob>();
        job2.SetupAdd(j => j.OnFaulted += It.IsAny<FlexiSphereJobExceptionHandler<Exception>>())
            .Callback(() => subscribed2 = true);

        IFlexiSphere tsphere = new FlexiSphere();

        tsphere.AddJob(job1.Object);
        tsphere.AddJob(job2.Object);

        tsphere.Jobs.Count.ShouldBe(2);
        subscribed1.ShouldBeTrue();
        subscribed2.ShouldBeTrue();
    }

    [Fact]
    public void AddTrigger_Null()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var action = () => tsphere.AddTrigger(null!);
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void AddTriggers()
    {
        bool onFaulted1 = false;
        bool onFaulted2 = false;
        bool onCompleted1 = false;
        bool onCompleted2 = false;
        bool onCanceled1 = false;
        bool onCanceled2 = false;
        bool onTriggered1 = false;
        bool onTriggered2 = false;

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        trigger1.SetupAdd(j => j.OnFaulted += It.IsAny<FlexiSphereTriggerExceptionHandler<FlexiSphereException>>())
            .Callback(() => onFaulted1 = true);
        trigger1.SetupAdd(j => j.OnCanceled += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => onCanceled1 = true);
        trigger1.SetupAdd(j => j.OnCompleted += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => onCompleted1 = true);
        trigger1.SetupAdd(j => j.OnTriggered += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => onTriggered1 = true);

        var trigger2 = new Mock<IFlexiSphereTrigger>();
        trigger2.SetupAdd(j => j.OnFaulted += It.IsAny<FlexiSphereTriggerExceptionHandler<FlexiSphereException>>())
            .Callback(() => onFaulted2 = true);
        trigger2.SetupAdd(j => j.OnCanceled += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => onCanceled2 = true);
        trigger2.SetupAdd(j => j.OnCompleted += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => onCompleted2 = true);
        trigger2.SetupAdd(j => j.OnTriggered += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => onTriggered2 = true);

        IFlexiSphere tsphere = new FlexiSphere();

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddTrigger(trigger2.Object);

        tsphere.Triggers.Count.ShouldBe(2);
        onFaulted1.ShouldBeTrue();
        onFaulted2.ShouldBeTrue();
        onCanceled1.ShouldBeTrue();
        onCanceled2.ShouldBeTrue();
        onCompleted1.ShouldBeTrue();
        onCompleted2.ShouldBeTrue();
        onTriggered1.ShouldBeTrue();
        onTriggered2.ShouldBeTrue();
    }

    [Fact]
    public async Task Start_With_Settings()
    {
        // Arrange
        bool subscribedTriggerFaulted = false;
        bool subscribedTriggerCanceled = false;
        bool subscribedTriggerCompleted = false;
        bool subscribedTriggerTriggered = false;
        bool subscribedJ = false;
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        trigger1.SetupAdd(j => j.OnFaulted += It.IsAny<FlexiSphereTriggerExceptionHandler<FlexiSphereException>>())
            .Callback(() => subscribedTriggerFaulted = true);
        trigger1.SetupAdd(j => j.OnCanceled += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => subscribedTriggerCanceled = true);
        trigger1.SetupAdd(j => j.OnCompleted += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => subscribedTriggerCompleted = true);
        trigger1.SetupAdd(j => j.OnTriggered += It.IsAny<FlexiSphereTriggerEventHandler>())
            .Callback(() => subscribedTriggerTriggered = true);

        var job1 = new Mock<IFlexiSphereJob>();
        job1.SetupAdd(j => j.OnFaulted += It.IsAny<FlexiSphereJobExceptionHandler<Exception>>())
            .Callback(() => subscribedJ = true);

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        tsphere.Triggers.Count.ShouldBe(1);
        tsphere.Jobs.Count.ShouldBe(1);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);

        subscribedJ.ShouldBeTrue();
        subscribedTriggerFaulted.ShouldBeTrue();
        subscribedTriggerCanceled.ShouldBeTrue();
        subscribedTriggerCompleted.ShouldBeTrue();
        subscribedTriggerTriggered.ShouldBeTrue();
    }

    [Fact]
    public async Task Start_Trigger_WithTriggeredRaised()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        var job1 = new Mock<IFlexiSphereJob>();
        job1.SetupGet(j => j.IsEnabled).Returns(true);

        var isFaulted = false;
        tsphere.OnFaulted += (sender, context, exception) => isFaulted = true;

        var isCanceled = false;
        tsphere.OnCanceled += (sender, context) => isCanceled = true;

        var isCompleted = false;
        tsphere.OnCompleted += (sender, context) => isCompleted = true;

        var isTriggered = false;
        tsphere.OnTriggered += (sender, context) => isTriggered = true;

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        trigger1.Raise(j => j.OnTriggered += null, trigger1.Object, null!);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        job1.Verify(j => j.ExecuteAsync(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);

        isCanceled.ShouldBeFalse();
        isCompleted.ShouldBeFalse();
        isTriggered.ShouldBeTrue();
        isFaulted.ShouldBeFalse();
    }

    [Fact]
    public async Task Start_Trigger_WithCancelRaised()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        var job1 = new Mock<IFlexiSphereJob>();

        var isFaulted = false;
        tsphere.OnFaulted += (sender, context, exception) => isFaulted = true;

        var isCanceled = false;
        tsphere.OnCanceled += (sender, context) => isCanceled = true;

        var isCompleted = false;
        tsphere.OnCompleted += (sender, context) => isCompleted = true;

        var isTriggered = false;
        tsphere.OnTriggered += (sender, context) => isTriggered = true;

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        job1.Verify(j => j.ExecuteAsync(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Never);

        trigger1.Raise(j => j.OnCanceled += null, trigger1.Object, null!);

        isCanceled.ShouldBeTrue();
        isCompleted.ShouldBeFalse();
        isTriggered.ShouldBeFalse();
        isFaulted.ShouldBeFalse();
    }

    [Fact]
    public async Task Start_Trigger_WithCompletedRaised()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        var job1 = new Mock<IFlexiSphereJob>();

        var isFaulted = false;
        tsphere.OnFaulted += (sender, context, exception) => isFaulted = true;

        var isCanceled = false;
        tsphere.OnCanceled += (sender, context) => isCanceled = true;

        var isCompleted = false;
        tsphere.OnCompleted += (sender, context) => isCompleted = true;

        var isTriggered = false;
        tsphere.OnTriggered += (sender, context) => isTriggered = true;

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        job1.Verify(j => j.ExecuteAsync(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Never);

        trigger1.Raise(j => j.OnCompleted += null, trigger1.Object, null!);

        isCanceled.ShouldBeFalse();
        isCompleted.ShouldBeTrue();
        isTriggered.ShouldBeFalse();
        isFaulted.ShouldBeFalse();
    }

    [Fact]
    public async Task Start_Trigger_WithFaultedRaised()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        var job1 = new Mock<IFlexiSphereJob>();

        var isFaulted = false;
        tsphere.OnFaulted += (sender, context, exception) => isFaulted = true;

        var isCanceled = false;
        tsphere.OnCanceled += (sender, context) => isCanceled = true;

        var isCompleted = false;
        tsphere.OnCompleted += (sender, context) => isCompleted = true;

        var isTriggered = false;
        tsphere.OnTriggered += (sender, context) => isTriggered = true;

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        job1.Verify(j => j.ExecuteAsync(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Never);

        trigger1.Raise(j => j.OnFaulted += null, trigger1.Object, null!, new FlexiSphereException());

        isCanceled.ShouldBeFalse();
        isCompleted.ShouldBeFalse();
        isTriggered.ShouldBeFalse();
        isFaulted.ShouldBeTrue();
    }

    [Fact]
    public async Task Start_Job_WithFaultedRaised()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        var job1 = new Mock<IFlexiSphereJob>();

        var isFaulted = false;
        tsphere.OnFaulted += (sender, context, exception) => isFaulted = true;

        var isCanceled = false;
        tsphere.OnCanceled += (sender, context) => isCanceled = true;

        var isCompleted = false;
        tsphere.OnCompleted += (sender, context) => isCompleted = true;

        var isTriggered = false;
        tsphere.OnTriggered += (sender, context) => isTriggered = true;

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        job1.Verify(j => j.ExecuteAsync(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Never);

        job1.Raise(j => j.OnFaulted += null, job1.Object, null!, new Exception());

        isCanceled.ShouldBeFalse();
        isCompleted.ShouldBeFalse();
        isTriggered.ShouldBeFalse();
        isFaulted.ShouldBeTrue();
    }

    [Theory]
    [InlineData("ontriggered")]
    [InlineData("oncanceled")]
    [InlineData("oncompleted")]
    [InlineData("onfaulted")]
    public async Task Start_Trigger_Exception_WhenOn(string action)
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();
        var job1 = new Mock<IFlexiSphereJob>();
        job1.SetupGet(j => j.IsEnabled).Returns(true);

        var isFaulted = 0;
        tsphere.OnFaulted += (sender, context, exception) =>
        {
            isFaulted++;
            if (action == "onfaulted")
            {
                throw new FlexiSphereException("Test");
            }
        };

        var isCanceled = false;
        tsphere.OnCanceled += (sender, context) =>
        {
            isCanceled = true;
            if (action == "oncanceled")
            {
                throw new Exception("Test");
            }
        };

        var isCompleted = false;
        tsphere.OnCompleted += (sender, context) =>
        {
            isCompleted = true;
            if (action == "oncompleted")
            {
                throw new Exception("Test");
            }
        };

        var isTriggered = false;
        tsphere.OnTriggered += (sender, context) =>
        {
            isTriggered = true;
            if (action == "ontriggered")
            {
                throw new Exception("Test");
            }
        };

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);

        if (action == "onfaulted")
        {
            trigger1.Raise(j => j.OnFaulted += null, trigger1.Object, null!, new FlexiSphereException());
        }
        else if (action == "oncanceled")
        {
            trigger1.Raise(j => j.OnCanceled += null, trigger1.Object, null!);
        }
        else if (action == "oncompleted")
        {
            trigger1.Raise(j => j.OnCompleted += null, trigger1.Object, null!);
        }
        else if (action == "ontriggered")
        {
            trigger1.Raise(j => j.OnTriggered += null, trigger1.Object, null!);
        }

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        if (action == "ontriggered")
        {
            job1.Verify(j => j.ExecuteAsync(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        isCanceled.ShouldBe(action == "oncanceled");
        isCompleted.ShouldBe(action == "oncompleted");
        isTriggered.ShouldBe(action == "ontriggered");
        isFaulted.ShouldBe(1);
    }

    [Fact]
    public async Task StopAsync_WithoutStart()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();

        var job1 = new Mock<IFlexiSphereJob>();

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StopAsync("test", cancellationToken: TestContext.Current.CancellationToken);

        tsphere.Triggers.Count.ShouldBe(1);
        tsphere.Jobs.Count.ShouldBe(1);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Never);
        trigger1.Verify(j => j.DeactivateTrigger(It.IsAny<string>(), It.IsAny<IFlexiSphereContext>()), Times.Once);
    }

    [Fact]
    public async Task StopAsync_Ok()
    {
        // Arrange
        IFlexiSphere tsphere = new FlexiSphere();

        var trigger1 = new Mock<IFlexiSphereTrigger>();

        var job1 = new Mock<IFlexiSphereJob>();

        tsphere.AddTrigger(trigger1.Object);
        tsphere.AddJob(job1.Object);

        await tsphere.StartAsync(cancellationToken: TestContext.Current.CancellationToken);
        await tsphere.StopAsync("test", cancellationToken: TestContext.Current.CancellationToken);

        tsphere.Triggers.Count.ShouldBe(1);
        tsphere.Jobs.Count.ShouldBe(1);

        trigger1.Verify(j => j.ActivateTrigger(It.IsAny<IFlexiSphereContext>(), It.IsAny<CancellationToken>()), Times.Once);
        trigger1.Verify(j => j.DeactivateTrigger(It.IsAny<string>(), It.IsAny<IFlexiSphereContext>()), Times.Once);
    }
}
