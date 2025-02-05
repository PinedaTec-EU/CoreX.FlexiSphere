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
// FileName: FlexiSphereFactoryTest.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.096Z
//
// --------------------------------------------------------------------------------------

#endregion

using Moq;

using Shouldly;

namespace ark.FlexiSphere.test;

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
