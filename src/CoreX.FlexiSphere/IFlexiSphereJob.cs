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
// FileName: IFlexiSphereJob.cs
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

namespace CoreX.FlexiSphere;

public interface IFlexiSphereJob : IFlexiSphereComponent
{
    /// <summary>
    /// Event triggered when the job is faulted
    /// </summary>
    event FlexiSphereJobExceptionHandler<Exception>? OnFaulted;

    /// <summary>
    /// Gets the unique identifier of the job
    /// </summary>
    Ulid Id { get; }

    /// <summary>
    /// Gets or sets the maximum number of concurrent executions
    /// </summary>
    int MaxConcurrents { get; set; }

    /// <summary>
    /// Gets or sets the name of the job
    /// </summary>
    string JobName { get; set; }

    /// <summary>
    /// Gets or sets the group of the job
    /// </summary>
    string? JobGroup { get; set; }

    /// <summary>
    /// Gets or sets the enabled state of the job
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the enabled state of the rate limiter
    /// </summary>
    bool IsRateLimiterEnabled { get; set; }

    /// <summary>
    /// Configures the job with the specified parameters
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobGroup"></param>
    /// <param name="maxConcurrents"></param>
    /// <param name="rateLimiter"></param>
    void ConfigureJob(string jobName, string? jobGroup, int maxConcurrents = 1, TimeSpan? rateLimiter = null);

    /// <summary>
    /// Configures the job with the specified parameters
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobGroup"></param>
    /// <param name="jobAction"></param>
    /// <param name="maxConcurrents"></param>
    /// <param name="rateLimiter"></param>
    void ConfigureJob(string jobName, string? jobGroup, Func<IFlexiSphereContext?, Task> jobAction, int maxConcurrents = 1, TimeSpan? rateLimiter = null);

    /// <summary>
    /// Executes the job
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExecuteAsync(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default);
}

public delegate void FlexiSphereJobExceptionHandler<T>(IFlexiSphereJob sender, IFlexiSphereContext? context, T exception) where T : Exception;