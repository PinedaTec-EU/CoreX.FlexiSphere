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
// FileName: FlexiSphereEventTrigger.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.093Z
//
// --------------------------------------------------------------------------------------

#endregion

using NLog;

namespace FlexiSphere.triggers;

public class FlexiSphereEventTrigger : FlexiSphereTriggerBase, IFlexiSphereEventTrigger
{
    private Func<IFlexiSphereContext?, Task<bool>>? _eventAction;

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ConfigureTrigger(Func<IFlexiSphereContext?, Task<bool>> eventAction, int delay = 1000,
        int maxConcurrent = 1, int maxOccurences = 0)
    {
        base.ConfigureTrigger(delay, maxConcurrent, maxOccurences);

        _eventAction = eventAction;
    }

    public override void ActivateTrigger(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        base.ActivateTrigger(context, cancellationToken);

        _eventAction.ThrowExceptionIfNull($"{nameof(_eventAction)} cannot be null!");
    }


    protected override async Task TimerCallback(object? state)
    {
        try
        {
            var cancellationToken = (CancellationToken)state!;
            if (cancellationToken.IsCancellationRequested)
            {
                this.DeactivateTrigger("Cancellation requested!");

                base.RaiseOnFlexiSphereTriggerCanceled(base._context);
                return;
            }

            if (this.MaxOccurrences > 0 && this.Counter >= this.MaxOccurrences)
            {
                this.DeactivateTrigger("Max occurrences reached!");

                base.RaiseOnFlexiSphereTriggerCompleted(base._context);
                return;
            }

            _logger.Info($"TimerCallback: {0} occurrences", _semaphore!.CurrentCount);

            await _semaphore!.WaitAsync();

            base.IncrementCounter();
            var result = await _eventAction!.Invoke(base._context);

            if (result)
            {
                base.RaiseOnFlexiSphereTriggered(base._context);
            }
        }
        catch (Exception ex)
        {
            base.RaiseOnFlexiSphereTriggerFaulted(base._context, ex);
        }
        finally
        {
            _semaphore!.Release();
        }
    }
}
