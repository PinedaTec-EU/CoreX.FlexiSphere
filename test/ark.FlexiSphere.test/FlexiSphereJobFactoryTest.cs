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

using Microsoft.Extensions.DependencyInjection;

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
        var action = () => factory.SetOwner(new Mock<IFlexiSphereComponentFactory>().Object);

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
    public void Factory_Setup_WithJobAction_JobType()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            return Task.FromResult(true);
        });

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .SetJobAction(jobAction)
            .DefineJob(typeof(FlexiSphereJob))
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }


    [Fact]
    public void Factory_Setup_WithJobType_JobAction()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            return Task.FromResult(true);
        });

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob(typeof(FlexiSphereJob))
            .SetJobAction(jobAction)
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }


    [Fact]
    public void Factory_Setup_WithJobAction_Null()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            return Task.FromResult(true);
        });

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .SetJobAction(null!)
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Setup_WithJobAction_JobInstance()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            return Task.FromResult(true);
        });

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .SetJobAction(jobAction)
            .DefineJob(new FlexiSphereJob())
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Setup_WithJobInstance_JobAction()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            return Task.FromResult(true);
        });

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob(new FlexiSphereJob())
            .SetJobAction(jobAction)
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Setup_JobInstance_JobType()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var jobAction = new Func<IFlexiSphereContext?, Task<bool>>((context) =>
        {
            return Task.FromResult(true);
        });

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob(typeof(FlexiSphereJob))
            .DefineJob(new FlexiSphereJob())
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Setup_WithJobInstance_Null()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        var mockJob = new Mock<IFlexiSphereJob>();

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob<FlexiSphereJob>(null!)
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
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

    [Fact]
    public void Factory_Setup_WithJobType_Null()
    {
        // Arrange
        var jobFactory = FlexiSphereJobFactory.Create();
        var jobName = "TestJob";
        var jobGroup = "TestGroup";
        var maxOccurences = 5;

        // Act
        var action = () => jobFactory.WithJobName(jobName, jobGroup)
            .SetMaxConcurrents(maxOccurences)
            .DefineJob(null!)
            .Build();

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void Factory_Setup_WithDirect_Options()
    {
        // Arrange
        FlexiSphereJobFactoryOptions options = new();
        options.MaxConcurrents = 5;

        var jobFactory = new FlexiSphereJobFactory(options);

        // Act
        var job = jobFactory.WithJobName("TestJob", "TestGroup")
            .SetJobAction(context => Task.FromResult(true))
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.MaxConcurrents.ShouldBe(5);
    }

    [Fact]
    public void Factory_Setup_WithDI()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere(options =>
        {
            options.JobFactoryOptions = new();
            options.JobFactoryOptions.MaxConcurrents = 5;
        });

        var serviceProvider = services.BuildServiceProvider();
        var jobFactory = serviceProvider.GetRequiredService<IFlexiSphereJobFactory>();

        // Act
        var job = jobFactory.WithJobName("TestJob", "TestGroup")
            .SetJobAction(context => Task.FromResult(true))
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.MaxConcurrents.ShouldBe(5);
    }

    [Fact]
    public void Factory_Setup_WithDI_JobFactory_CustomOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere<FakeClass_FlexiSphereJobFactory>(options =>
        {
            options.JobFactoryOptions = new();
            options.JobFactoryOptions.MaxConcurrents = 5;
        });

        var serviceProvider = services.BuildServiceProvider();
        var jobFactory = serviceProvider.GetRequiredService<IFlexiSphereJobFactory>();

        // Act
        var job = jobFactory.WithJobName("TestJob", "TestGroup")
            .SetJobAction(context => Task.FromResult(true))
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.ShouldBeOfType<FakeClass_FlexiSphereJob>();
    }

    [Fact]
    public void Factory_Setup_WithDI_JobFactory_NoOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFlexiSphere<FakeClass_FlexiSphereJobFactory>();

        var serviceProvider = services.BuildServiceProvider();
        var jobFactory = serviceProvider.GetRequiredService<IFlexiSphereJobFactory>();

        // Act
        var job = jobFactory.WithJobName("TestJob", "TestGroup")
            .SetJobAction(context => Task.FromResult(true))
            .Build();

        // Assert
        job.ShouldNotBeNull();
        job.ShouldBeOfType<FakeClass_FlexiSphereJob>();
    }
}
