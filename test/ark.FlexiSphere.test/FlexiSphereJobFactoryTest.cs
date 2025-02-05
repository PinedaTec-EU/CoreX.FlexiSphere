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
// FileName: FlexiSphereJobFactoryTest.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.097Z
//
// --------------------------------------------------------------------------------------

#endregion

using ark.FlexiSphere.jobs;

using Moq;

using Shouldly;

namespace ark.FlexiSphere.test;

public class FlexiSphereJobFactoryTest
{
    [Fact]
    public void Factory_WithOut_Settings()
    {
        // Arrange
        var factory = FlexiSphereJobFactory.Create();
        // Act
        var action = () => factory.Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_With_SetOwner()
    {
        // Arrange
        var factory = FlexiSphereJobFactory.Create();

        // Act
        var action = () => factory.SetOwner(new Mock<IFlexiSphereFactory>().Object);

        // Assert
        action.ShouldNotThrow();
    }

    [Fact]
    public void Factory_WithRateLimiter()
    {
        // Arrange
        var factory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;
        var rateLimiter = TimeSpan.FromSeconds(5);

        // Act
        var job = factory.WithJobName(jobName, jobGroup)
            .SetRateLimiter(rateLimiter, maxOccurences)
            .SetJobAction(context => Task.FromResult(true))
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.JobName.ShouldBe(jobName);
        job.JobGroup.ShouldBe(jobGroup);
        job.MaxConcurrents.ShouldBe(maxOccurences);
        job.IsRateLimiterEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Factory_Without_JobAction()
    {
        // Arrange
        var factory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;
        var rateLimiter = TimeSpan.FromSeconds(5);

        // Act
        var action = () => factory.WithJobName(jobName, jobGroup)
            .SetRateLimiter(rateLimiter, maxOccurences)
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void WithJobName_WhenJobNameIsNull_ShouldThrowException()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = null as string;
        var jobGroup = "TestGroup";

        // Act
        var action = () => jobFactory.WithJobName(jobName!, jobGroup);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void WithJobName_WhenJobGroupIsNullOrEmpty_NotThrowException()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = string.Empty;

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup);

        // Assert
        action.ShouldNotThrow();
    }

    [Fact]
    public async Task Factory_Setup_WithJobAction()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        bool jobActionExecuted = false;
        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            jobActionExecuted = true;
            return Task.FromResult(true);
        });

        // Act
        var job = jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .SetJobAction(jobAction)
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.JobName.ShouldBe(jobName);
        job.JobGroup.ShouldBe(jobGroup);

        jobActionExecuted.ShouldBeFalse();

        await job.ExecuteAsync(null, TestContext.Current.CancellationToken);
        jobActionExecuted.ShouldBeTrue();
    }

    [Fact]
    public void Factory_Setup_WithJobInstance()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var mockJob = new Mock<IFlexiSphereJob>();

        // Act
        var job = jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob(mockJob.Object)
            .Build();

        // Assert
        job.ShouldNotBeNull();

        mockJob.Verify(j => j.ConfigureJob(jobName, jobGroup, maxOccurences, null), Times.Once);
    }

    [Fact]
    public void Factory_Setup_WithJobType()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        // Act
        var job = jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob(typeof(FlexiSphereJob))
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.ShouldBeOfType<FlexiSphereJob>();
        job.JobName.ShouldBe(jobName);
        job.JobGroup.ShouldBe(jobGroup);
        job.MaxConcurrents.ShouldBe(maxOccurences);

        job.IsRateLimiterEnabled.ShouldBeFalse();
    }
}
