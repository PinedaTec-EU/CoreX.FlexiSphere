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
// FileName: FlexiSphereTriggerBase.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.095Z
//
// --------------------------------------------------------------------------------------

#endregion

using ark.extensions;

using NLog;

namespace ark.FlexiSphere.triggers;

public abstract class FlexiSphereTriggerBase : IFlexiSphereTrigger
{
    public event FlexiSphereTriggerEventHandler? OnTriggered;
    public event FlexiSphereTriggerEventHandler? OnCanceled;
    public event FlexiSphereTriggerEventHandler? OnCompleted;
    public event FlexiSphereTriggerExceptionHandler<FlexiSphereException>? OnFaulted;

    public int MaxOccurrences { get; set; } = 0;
    public int MaxConcurrent { get; set; } = 1;
    public int Counter { get => _counter; }

    public string TriggerName { get; set; } = "FlexiSphereTrigger";

    public bool FireTriggerOnStart { get; set; } = false;

    public int PressureCounter => this.MaxConcurrent - _semaphore?.CurrentCount ?? 0;

    protected Timer? _timer;
    protected IFlexiSphereContext? _context;

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private int _counter = 0;
    private int _delay = 0;

    protected SemaphoreSlim? _semaphore = null;

    protected virtual void ConfigureTrigger(int delay = 1000, int maxConcurrent = 1, int maxOccurences = 0)
    {
        this.MaxConcurrent = maxConcurrent;
        this.MaxOccurrences = maxOccurences;

        _delay = delay;

        (this.MaxConcurrent < 1).ThrowExceptionIfTrue<ArgumentOutOfRangeException>(
            $"{nameof(this.MaxConcurrent)} must be greater than 0!");
        // (this.MaxOccurrences < 1).ThrowExceptionIfTrue<ArgumentOutOfRangeException>(nameof(maxOccurences),
        //     "The maximum number of occurrences must be greater than 0!");

        _logger.Info($"The trigger {this.TriggerName} has been configured with a delay of {_delay} ms, a maximum of {this.MaxConcurrent} " +
            $"concurrent executions and a maximum of {(this.MaxOccurrences != 0 ? this.MaxOccurrences : "unlimited")} occurrences.");
    }

    public virtual void ActivateTrigger(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        _logger.Info($"Activating trigger {this.TriggerName}...");

        _context = context;
        _semaphore = new SemaphoreSlim(this.MaxConcurrent);

        if (_delay == 0)
        {
            _logger.Warn("The delay is set to 0, the trigger will not be activated itself.");
            return;
        }

        _timer = new Timer(async (state) => await this.TimerCallback(state), cancellationToken, _delay, _delay);
    }

    protected abstract Task TimerCallback(object? state);

    protected void IncrementCounter()
    {
        Interlocked.Increment(ref _counter);
    }

    public void DeactivateTrigger(string issue, IFlexiSphereContext? context = null)
    {
        _logger.Warn($"Deactivating trigger {this.TriggerName}... [{issue}]");

        this._timer?.Change(Timeout.Infinite, Timeout.Infinite);
        this._timer?.Dispose();

        this.OnCompleted?.Invoke(this, context);
    }

    protected void RaiseOnFlexiSphereTriggered(IFlexiSphereContext? context)
    {
        _ = Task.Run(() =>
        {
            try
            {
                this.OnTriggered?.Invoke(this, context);
            }
            catch (Exception ex)
            {
                this.RaiseOnFlexiSphereTriggerFaulted(context, ex);
            }
        });
    }

    protected void RaiseOnFlexiSphereTriggerFaulted(IFlexiSphereContext? context, Exception exception)
    {
        _ = Task.Run(() =>
        {
            try
            {
                if (exception is FlexiSphereException fsException)
                {
                    this.OnFaulted?.Invoke(this, context, fsException);
                }
                else
                {
                    this.OnFaulted?.Invoke(this, context, new FlexiSphereException("An error occurred while executing the trigger", exception));
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "An error occurred while raising the OnFlexiSphereTriggerFaulted event");
            }
        });
    }

    protected void RaiseOnFlexiSphereTriggerCanceled(IFlexiSphereContext? context)
    {
        _ = Task.Run(() =>
        {
            try
            {
                this.OnCanceled?.Invoke(this, context);
            }
            catch (Exception ex)
            {
                this.RaiseOnFlexiSphereTriggerFaulted(context, ex);
            }
        });
    }

    protected void RaiseOnFlexiSphereTriggerCompleted(IFlexiSphereContext? context)
    {
        _ = Task.Run(() =>
        {
            try
            {
                this.OnCompleted?.Invoke(this, context);
            }
            catch (Exception ex)
            {
                this.RaiseOnFlexiSphereTriggerFaulted(context, ex);
            }
        });
    }
}
