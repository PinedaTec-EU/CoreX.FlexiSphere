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
// FileName: FlexiSphereJobTest.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.098Z
//
// --------------------------------------------------------------------------------------

#endregion

using CoreX.FlexiSphere.jobs;

using Shouldly;

namespace CoreX.FlexiSphere.test;

public class FlexiSphereJobTest : IClassFixture<TestFixture>
{
    private readonly TestFixture _testFixture;

    public FlexiSphereJobTest(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact]
    public void ConfigureJob_WhenJobNameIsNullOrEmpty_ShouldThrowException()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = string.Empty;

        // Act
        var action = () => job.ConfigureJob(jobName, null, null!);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void ConfigureJob_WhenJobActionIsNull_ShouldThrowException()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";

        // Act
        var action = () => job.ConfigureJob(jobName, null, null!);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void ConfigureJob_WhenMaxConcurrentIsLessThanOne_ShouldThrowException()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";
        var jobAction = new Func<IFlexiSphereContext?, Task>(context => Task.CompletedTask);
        var maxConcurrent = 0;

        // Act
        var action = () => job.ConfigureJob(jobName, null, jobAction, maxConcurrent);

        // Assert
        action.ShouldThrow<FlexiSphereException>();
    }

    [Fact]
    public void ConfigureJob_WithRateLimiter()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";
        var jobAction = new Func<IFlexiSphereContext?, Task>(context => Task.CompletedTask);
        var maxConcurrent = 1;
        var rateLimiter = TimeSpan.FromSeconds(1);

        // Act
        job.ConfigureJob(jobName, null, jobAction, maxConcurrent, rateLimiter);

        // Assert
        job.IsRateLimiterEnabled.ShouldBeTrue();
    }

    [Fact]
    public void ConfigureJob_WithoutRateLimiter()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";
        var jobAction = new Func<IFlexiSphereContext?, Task>(context => Task.CompletedTask);
        var maxConcurrent = 1;
        var rateLimiter = TimeSpan.FromSeconds(1);

        // Act
        job.ConfigureJob(jobName, null, jobAction, maxConcurrent);

        // Assert
        job.IsRateLimiterEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_WhenException_ShouldRaiseOnJobFaulted()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";
        var jobAction = new Func<IFlexiSphereContext?, Task>(context => throw new Exception());
        job.ConfigureJob(jobName, null, jobAction);

        bool isFaulted = false;
        job.OnFaulted += (sender, context, exception) => isFaulted = true;

        // Act
        var action = () => job.ExecuteAsync(null, TestContext.Current.CancellationToken);

        // Assert
        await action.ShouldNotThrowAsync();
        isFaulted.ShouldBeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WhenException_OnFaulted()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";
        var jobAction = new Func<IFlexiSphereContext?, Task>(context => throw new Exception());
        job.ConfigureJob(jobName, null, jobAction);

        bool isFaulted = false;
        job.OnFaulted += (sender, context, exception) =>
        {
            isFaulted = true;
            throw new Exception();
        };

        // Act
        var action = () => job.ExecuteAsync(null, TestContext.Current.CancellationToken);

        // Assert
        await action.ShouldNotThrowAsync();
        isFaulted.ShouldBeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithRateLimiterEnabled()
    {
        // Arrange
        IFlexiSphereJob job = new FlexiSphereJob();
        var jobName = "TestJob";
        var actionExec = false;
        var jobAction = new Func<IFlexiSphereContext?, Task>(context =>
        {
            actionExec = true;
            return Task.CompletedTask;
        });
        var maxConcurrent = 1;
        var rateLimiter = TimeSpan.FromSeconds(1);
        job.ConfigureJob(jobName, null, jobAction, maxConcurrent, rateLimiter);

        // Act
        await job.ExecuteAsync(null, TestContext.Current.CancellationToken);

        // Assert
        actionExec.ShouldBeTrue();
    }
}
