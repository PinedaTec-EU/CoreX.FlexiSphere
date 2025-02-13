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
// FileName: IFlexiSphere.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.085Z
//
// --------------------------------------------------------------------------------------

#endregion

namespace CoreX.FlexiSphere;

public interface IFlexiSphere : IFlexiSphereComponent
{
    /// <summary>
    /// Event triggered when the FlexiSphere is canceled.
    /// </summary>
    event FlexiSphereHandler? OnCanceled;

    /// <summary>
    /// Event triggered when the FlexiSphere has completed a flow. 
    /// </summary>
    event FlexiSphereHandler? OnCompleted;

    /// <summary>
    /// Event triggered when the FlexiSphere is triggered.
    /// </summary>
    event FlexiSphereHandler? OnTriggered;

    /// <summary>
    /// Event triggered when the FlexiSphere is faulted.
    /// </summary>
    event FlexiSphereExceptionHandler<Exception>? OnFaulted;

    /// <summary>
    /// Event triggered before a job is executed.
    /// </summary>
    event FlexiSphereHandler? OnBeforeJobExecuted;

    /// <summary>
    /// Event triggered after a job is executed.
    /// </summary>
    event FlexiSphereHandler? OnAfterJobExecuted;

    /// <summary>
    /// Gets the counter of the FlexiSphere executions.
    /// </summary>
    int Counter { get; }

    /// <summary>
    /// Gets the last time the FlexiSphere was triggered.
    /// </summary>
    DateTime? LastTriggered { get; }

    /// <summary>
    /// Gets the list of FlexiSphere triggers.
    /// </summary>
    IReadOnlyCollection<IFlexiSphereTrigger> Triggers { get; }

    /// <summary>
    /// Gets the list of FlexiSphere jobs.
    /// </summary>
    IReadOnlyCollection<IFlexiSphereJob> Jobs { get; }

    /// <summary>
    /// Adds a trigger to the FlexiSphere.
    /// </summary>
    /// <param name="trigger"></param>
    void AddTrigger(IFlexiSphereTrigger trigger);

    /// <summary>
    /// Adds a job to the FlexiSphere.
    /// </summary>
    /// <param name="job"></param>
    void AddJob(IFlexiSphereJob job);

    /// <summary>
    /// Starts the FlexiSphere.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the FlexiSphere.
    /// </summary>
    /// <param name="issue"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StopAsync(string issue, IFlexiSphereContext? context = null, CancellationToken cancellationToken = default);
}
