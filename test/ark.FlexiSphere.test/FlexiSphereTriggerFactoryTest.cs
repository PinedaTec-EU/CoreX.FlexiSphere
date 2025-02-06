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
// FileName: FlexiSphereTriggerFactoryTest.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.101Z
//
// --------------------------------------------------------------------------------------

#endregion

using ark.FlexiSphere.triggers;
using Microsoft.Extensions.DependencyInjection;
using Moq;

using Shouldly;

namespace ark.FlexiSphere.test;

public class FlexiSphereTriggerFactoryTest
{
    [Fact]
    public void Factory_WithOut_Settings()
    {
        // Arrange
        var factory = FlexiSphereTriggerFactory.Create();
        // Act
        var action = () => factory.Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_With_SetOwner()
    {
        // Arrange
        var factory = FlexiSphereTriggerFactory.Create();

        // Act
        var action = () => factory.SetOwner(new Mock<IFlexiSphereComponentFactory>().Object);

        // Assert
        action.ShouldNotThrow();
    }

    #region EventTrigger

    [Fact]
    public void Factory_EventTrigger()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        factory
            .WithTriggerName("Test", "TestGroup")
            .SetMaxOccurences(5)
            .SetMaxConcurrent(7)
            .ActivateOnAction((context) => Task.FromResult(true));

        // Act
        var trigger = factory.Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.MaxOccurrences.ShouldBe(5);
        trigger.MaxConcurrents.ShouldBe(7);

        trigger.TriggerName.ShouldBe("Test");
    }

    [Fact]
    public void Factory_EventTrigger_FireOnStart()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        factory
            .FireTriggerOnStart(true)
            .WithTriggerName("Test", "TestGroup")
            .SetMaxOccurences(5)
            .SetMaxConcurrent(7)
            .ActivateOnAction((context) => Task.FromResult(true));

        // Act
        var trigger = factory.Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.MaxOccurrences.ShouldBe(5);
        trigger.MaxConcurrents.ShouldBe(7);

        trigger.TriggerName.ShouldBe("Test");
        trigger.FireTriggerOnStart.ShouldBeTrue();
    }


    [Fact]
    public void Factory_EventTrigger_InvalidMaxConcurrent()
    {
        var context = new Mock<IFlexiSphereContext>();
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        factory.SetMaxOccurences(5)
            .SetMaxConcurrent(0)
            .ActivateOnAction((context) => Task.FromResult(true));

        // Act
        var action = () => factory.Build();

        // Assert
        action.ShouldThrow<ArgumentException>();
    }

    #endregion

    #region ScheduledTrigger

    [Fact]
    public void Factory_ScheduledTrigger()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        factory
            .FireTriggerOnStart(true)
            .StartOn("0 0/20 * * * *")
            .SetMaxConcurrent(5)
            .SetMaxOccurences(7);

        // Act
        var trigger = factory.Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.MaxOccurrences.ShouldBe(7);
        trigger.MaxConcurrents.ShouldBe(5);

        var triggerScheduled = trigger as FlexiSphereScheduledTrigger;
        triggerScheduled.ShouldNotBeNull();
        triggerScheduled!.CronTime.ShouldBe("0,20,40 * * * *");
    }

    [Fact]
    public void Factory_ScheduledTrigger_InvalidaMaxConcurrent()
    {
        var context = new Mock<IFlexiSphereContext>();
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        factory.StartOn("0 0/20 * * * *")
            .SetMaxConcurrent(0)
            .SetMaxOccurences(7);

        // Act
        var action = () => factory.Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_ScheduledTrigger_InvalidCronExpression()
    {
        var context = new Mock<IFlexiSphereContext>();
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        factory.StartOn("abcdefgh")
            .SetMaxConcurrent(5)
            .SetMaxOccurences(7);

        // Act
        var action = () => factory.Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    #endregion

    [Fact]
    public void Factory_InvalidSettings_SA()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        var action = () => factory.StartOn("0 0/20 * * * *")
            .ActivateOnAction((context) => Task.FromResult(true));

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_InvalidSettings_AS()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        var action = () => factory.ActivateOnAction((context) => Task.FromResult(true))
            .StartOn("0 0/20 * * * *");

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Invalid_ActivateOnAction()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        var action = () => factory.StartOn("0 0/20 * * * *")
             .ActivateOnAction(null!);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Duplicated_ActivateOnAction()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        var action = () => factory.ActivateOnAction((context) => Task.FromResult(true))
            .ActivateOnAction((context) => Task.FromResult(true));

        // Assert
        action.ShouldNotThrow();
    }

    [Fact]
    public void Factory_Duplicated_StartOn()
    {
        // Arrange
        var factory = new FlexiSphereTriggerFactory();
        var action = () => factory.StartOn("0 0/20 * * * *")
            .StartOn("0 0/20 * * * *");

        // Assert
        action.ShouldNotThrow();
    }

    [Fact]
    public void Factory_Setup_WithDirect_Options()
    {
        // Arrange
        FlexiSphereTriggerFactoryOptions options = new();
        options.MaxConcurrents = 5;

        var triggerFactory = new FlexiSphereTriggerFactory(options);

        // Act
        var trigger = triggerFactory
            .ActivateOnAction((context) => Task.FromResult(true))
            .WithTriggerName("TestTrigger", "TestGroup")
            .FireTriggerOnStart(true)
            .Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.MaxConcurrents.ShouldBe(5);
    }

    [Fact]
    public void Factory_Event_Setup_WithDI()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere(options =>
        {
            options.TriggerFactoryOptions = new();
            options.TriggerFactoryOptions.MaxConcurrents = 5;
        });

        var serviceProvider = services.BuildServiceProvider();
        var triggerFactory = serviceProvider.GetRequiredService<IFlexiSphereTriggerFactory>();

        // Act
        var trigger = triggerFactory
            .WithTriggerName("TestTrigger", "TestGroup")
            .ActivateOnAction((context) => Task.FromResult(true))
            .FireTriggerOnStart(true)
            .Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.MaxConcurrents.ShouldBe(5);
    }

    [Fact]
    public void Factory_Scheduled_Setup_WithDI()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere(options =>
        {
            options.TriggerFactoryOptions = new();
            options.TriggerFactoryOptions.MaxConcurrents = 5;
        });

        var serviceProvider = services.BuildServiceProvider();
        var triggerFactory = serviceProvider.GetRequiredService<IFlexiSphereTriggerFactory>();

        // Act
        var trigger = triggerFactory
            .WithTriggerName("TestTrigger", "TestGroup")
            .StartOn("0 0/20 * * * *")
            .FireTriggerOnStart(true)
            .Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.MaxConcurrents.ShouldBe(5);
    }

    [Fact]
    public void Factory_Setup_WithDI_TriggerFactory_CustomOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere<FakeClass_FlexiSphereTriggerFactory>(options =>
        {
            options.TriggerFactoryOptions = new();
            options.TriggerFactoryOptions.MaxConcurrents = 5;
        });

        var serviceProvider = services.BuildServiceProvider();
        var triggerFactory = serviceProvider.GetRequiredService<IFlexiSphereTriggerFactory>();

        // Act
        var trigger = triggerFactory.WithTriggerName("TestTrigger", "TestGroup")
            .FireTriggerOnStart(true)
            .Build();

        // Assert
        trigger.ShouldNotBeNull();
        trigger.ShouldBeOfType<FakeClass_FlexiSphereTrigger>();
    }

    [Fact]
    public void Factory_Setup_WithDI_TriggerFactory_NoOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere<FakeClass_FlexiSphereTriggerFactory>();

        var serviceProvider = services.BuildServiceProvider();
        var TriggerFactory = serviceProvider.GetRequiredService<IFlexiSphereTriggerFactory>();

        // Act
        var Trigger = TriggerFactory.WithTriggerName("TestTrigger", "TestGroup")
            .FireTriggerOnStart(true)
            .Build();

        // Assert
        Trigger.ShouldNotBeNull();
        Trigger.ShouldBeOfType<FakeClass_FlexiSphereTrigger>();
    }
}
