using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexiSphere.jobs;
using Shouldly;

namespace FlexiSphere.test;

public class FlexiSphereJobTest
{
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
