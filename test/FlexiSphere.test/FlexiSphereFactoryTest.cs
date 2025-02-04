using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Shouldly;

namespace FlexiSphere.test;

public class FlexiSphereFactoryTest
{
    [Fact]
    public void CreateFactoryInstance()
    {
        // Arrange
        var Factory = FlexiSphereFactory.Create();

        // Act
        Factory.ShouldNotBeNull();
    }

    [Fact]
    public void Build()
    {
        // Arrange
        var Factory = FlexiSphereFactory.Create();

        // Act
        var tsphere = Factory.Build();

        // Assert
        tsphere.ShouldNotBeNull();
    }

    [Fact]
    public void AddJob()
    {
        // Arrange
        var Factory = FlexiSphereFactory.Create();
        var job = new Mock<IFlexiSphereJob>();

        // Act
        Factory.AddJob(job.Object);

        // Assert
        Factory.Build().Jobs.Count.ShouldBe(1);
    }

    [Fact]
    public void AddJobWithAction()
    {
        // Arrange
        var Factory = FlexiSphereFactory.Create();

        // Act
        Factory.AddJob(b => b.WithJobName("Test", "").SetJobAction((context) => { return Task.FromResult(true); }));

        // Assert
        Factory.Build().Jobs.Count.ShouldBe(1);
    }

    [Fact]
    public void AddTrigger()
    {
        // Arrange
        var Factory = FlexiSphereFactory.Create();
        var trigger = new Mock<IFlexiSphereTrigger>();

        // Act
        Factory.AddTrigger(trigger.Object);

        // Assert
        Factory.Build().Triggers.Count.ShouldBe(1);
    }

    [Fact]
    public void AddTriggerWithAction()
    {
        // Arrange
        var Factory = FlexiSphereFactory.Create();

        // Act
        Factory.AddTrigger(b => b.WithTriggerName("Test", "").StartOn("5 0 * 8 *"));

        // Assert
        Factory.Build().Triggers.Count.ShouldBe(1);
    }
}
