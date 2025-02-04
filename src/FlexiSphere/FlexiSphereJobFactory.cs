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
// FileName: FlexiSphereJobFactory.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.083Z
//
// --------------------------------------------------------------------------------------

#endregion

using FlexiSphere.jobs;

namespace FlexiSphere;

public class FlexiSphereJobFactory : IFlexiSphereJobFactory
{
    private string? _jobName;
    private string? _jobGroup;
    private int _maxConcurrents = 1;
    private TimeSpan? _rateLimiter;

    private IFlexiSphereJob? _jobInstance;
    private Func<IFlexiSphereContext?, Task>? _jobAction;

    public static IFlexiSphereJobFactory Create() =>
        new FlexiSphereJobFactory();

    public IFlexiSphereJobFactory SetOwner(IFlexiSphereFactory owner)
    {
        return this;
    }

    public IFlexiSphereJobFactory WithJobName(string jobName, string jobGroup)
    {
        jobName.ThrowExceptionIfNullOrEmpty<FlexiSphereException>($"{nameof(jobName)} cannot be null or empty!");

        _jobName = jobName;
        _jobGroup = jobGroup;
        return this;
    }

    public IFlexiSphereJobFactory SetMaxConcurrents(int maxConcurrents)
    {
        _maxConcurrents = maxConcurrents;
        _rateLimiter = null;

        return this;
    }

    public IFlexiSphereJobFactory SetJobAction(Func<IFlexiSphereContext?, Task> jobAction)
    {
        _jobAction = jobAction;
        return this;
    }

    public IFlexiSphereJobFactory SetRateLimiter(TimeSpan rateLimiter, int maxConcurrents)
    {
        this.SetMaxConcurrents(maxConcurrents);
        _rateLimiter = rateLimiter;

        return this;
    }

    public IFlexiSphereJobFactory DefineJob<TType>(TType jobInstance) where TType : class, IFlexiSphereJob
    {
        _jobInstance = jobInstance;
        return this;
    }

    public IFlexiSphereJobFactory DefineJob(Type jobType)
    {
        try
        {
            _jobInstance = Activator.CreateInstance(jobType) as IFlexiSphereJob;
            _jobInstance.ThrowExceptionIfNull<FlexiSphereException>($"Cannot create instance of {jobType.Name}");

            return this;
        }
        catch (Exception ex) when (ex is not FlexiSphereException)
        {
            throw new FlexiSphereException($"An error occurred while creating instance of {jobType.Name}", ex);
        }
    }

    public IFlexiSphereJob Build()
    {
        _jobName.ThrowExceptionIfNullOrEmpty<FlexiSphereException>($"{nameof(_jobName)} cannot be null or empty!");

        if (_jobInstance is not null)
        {
            return this.CreateJobInstance(_jobInstance);
        }

        var job = this.CreateJobInstance();

        return job;
    }

    private IFlexiSphereJob CreateJobInstance()
    {
        var job = new FlexiSphereJob();

        _jobAction.ThrowExceptionIfNull<FlexiSphereException>($"{nameof(_jobAction)} cannot be null!");

        job.ConfigureJob(_jobName!, _jobGroup, _jobAction!, _maxConcurrents, _rateLimiter);

        return job;
    }

    private IFlexiSphereJob CreateJobInstance<TType>(TType jobInstance) where TType : class, IFlexiSphereJob
    {
        var job = jobInstance;
        job.ConfigureJob(_jobName!, _jobGroup, _maxConcurrents);

        return job;
    }
}
