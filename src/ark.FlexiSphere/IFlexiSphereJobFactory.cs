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
// FileName: IFlexiSphereJobFactory.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.090Z
//
// --------------------------------------------------------------------------------------

#endregion

namespace ark.FlexiSphere;

public interface IFlexiSphereJobFactory
{
    /// <summary>
    /// Sets the owner of the job
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory SetOwner(IFlexiSphereFactory owner);

    /// <summary>
    /// Defines the job name and group
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobGroup"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory WithJobName(string jobName, string jobGroup);

    /// <summary>
    /// Sets the maximum number of concurrent executions
    /// </summary>
    /// <param name="maxConcurrent"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory SetMaxConcurrents(int maxConcurrent);

    /// <summary>
    /// Sets the rate limiter for the job
    /// </summary>
    /// <param name="rateLimiter"></param>
    /// <param name="maxConcurrents"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory SetRateLimiter(TimeSpan rateLimiter, int maxConcurrents);

    /// <summary>
    /// Sets the job action
    /// </summary>
    /// <param name="jobAction"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory SetJobAction(Func<IFlexiSphereContext?, Task> jobAction);

    /// <summary>
    /// Defines the job
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <param name="jobInstance"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory DefineJob<TType>(TType jobInstance) where TType : class, IFlexiSphereJob;

    /// <summary>
    /// Defines the job
    /// </summary>
    /// <param name="jobType"></param>
    /// <returns></returns>
    IFlexiSphereJobFactory DefineJob(Type jobType);

    /// <summary>
    /// Builds the job
    /// </summary>
    /// <returns></returns>
    IFlexiSphereJob Build();
}
