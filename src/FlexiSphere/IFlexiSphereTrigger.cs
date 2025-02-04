using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

public interface IFlexiSphereTrigger : IFlexiSphereComponent
{
    /// <summary>
    /// Event triggered when the FlexiSphereTrigger is triggered.
    /// </summary>
    event FlexiSphereTriggerEventHandler? OnTriggered;

    /// <summary>
    /// Event triggered when the FlexiSphereTrigger is canceled.
    /// </summary>
    event FlexiSphereTriggerEventHandler? OnCanceled;

    /// <summary>
    /// Event triggered when the FlexiSphereTrigger is completed.
    /// </summary>
    event FlexiSphereTriggerEventHandler? OnCompleted;

    /// <summary>
    /// Event triggered when the FlexiSphereTrigger is faulted.
    /// </summary>
    event FlexiSphereTriggerExceptionHandler<FlexiSphereException>? OnFaulted;

    /// <summary>
    /// Gets or sets if the trigger should be fired on start.
    /// </summary>
    bool FireTriggerOnStart { get; set; }

    /// <summary>
    /// Gets or sets the name of the trigger.
    /// </summary>
    string TriggerName { get; set; }

    /// <summary>
    /// Gets or sets the maximum occurrences of the trigger.
    /// </summary>
    int MaxOccurrences { get; set; }

    /// <summary>
    /// Gets or sets the maximum concurrent occurrences of the trigger.
    /// </summary>
    int MaxConcurrent { get; set; }

    /// <summary>
    /// Gets the counter of the trigger executions.
    /// </summary>
    int Counter { get; }

    /// <summary>
    /// Gets or sets the maximum pressure of the trigger.
    /// </summary>
    int PressureCounter { get; }

    /// <summary>
    /// Activates the trigger.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    void ActivateTrigger(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the trigger.
    /// </summary>
    /// <param name="issue"></param>
    /// <param name="context"></param>
    void DeactivateTrigger(string issue, IFlexiSphereContext? context = null);
}

public delegate void FlexiSphereTriggerEventHandler(IFlexiSphereTrigger sender, IFlexiSphereContext? context);
public delegate void FlexiSphereTriggerExceptionHandler<T>(IFlexiSphereTrigger sender, IFlexiSphereContext? context, T exception) where T : FlexiSphereException;
