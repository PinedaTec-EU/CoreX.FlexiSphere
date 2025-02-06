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
// FileName: FlexiSphereScheduledTrigger.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.094Z
//
// --------------------------------------------------------------------------------------

#endregion

using ark.extensions;

using Cronos;

using NLog;

namespace ark.FlexiSphere.triggers;

public class FlexiSphereScheduledTrigger : FlexiSphereTriggerBase, IFlexiSphereScheduledTrigger
{
    public string CronTime { get => _cronExpression?.ToString() ?? "0 0/5 * * * *"; }

    private CronExpression? _cronExpression;

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public void ConfigureTrigger(string? cronExpression, int maxConcurrent = 100, int maxOccurences = 0)
    {
        this.MaxConcurrent = maxConcurrent;
        this.MaxOccurrences = maxOccurences;

        try
        {
            (this.MaxConcurrent < 1).ThrowArgumentExceptionIfTrue(nameof(maxConcurrent),
                $"{nameof(this.MaxConcurrent)} must be greater than 0!");

            if (string.IsNullOrEmpty(cronExpression))
            {
                cronExpression = this.CronTime;
            }

            var format = CronFormat.IncludeSeconds;

            if (cronExpression.Split(' ').Length == 5)
            {
                format = CronFormat.Standard;
            }

            _cronExpression = CronExpression.Parse(cronExpression, format);
        }
        catch (Exception ex)
        {
            this.RaiseOnFlexiSphereTriggerFaulted(_context, ex);
            throw new FlexiSphereException("Error configuring trigger!", ex);
        }
    }

    public override async void ActivateTrigger(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        _logger.Info("Activating trigger...");
        _logger.Info("Activating trigger on start is {0}.", this.FireTriggerOnStart.ToEnabledDisabled());

        _context = context;
        _semaphore = new SemaphoreSlim(this.MaxConcurrent);

        _timer = new Timer(async (state) => await TimerCallback(state), cancellationToken, Timeout.Infinite, Timeout.Infinite);

        if (this.FireTriggerOnStart)
        {
            await TimerCallback(cancellationToken);
            return;
        }

        this.ScheduleNextExecution();
    }

    protected override async Task TimerCallback(object? state)
    {
        try
        {
            _logger.Info("TimerCallback: {0} occurrences", this.Counter);

            var cancellationToken = (CancellationToken)state!;
            if (cancellationToken.IsCancellationRequested)
            {
                this.DeactivateTrigger("Cancellation requested.");

                base.RaiseOnFlexiSphereTriggerCanceled(_context);
                return;
            }

            await _semaphore!.WaitAsync();

            if (this.MaxOccurrences > 0 && this.Counter >= this.MaxOccurrences)
            {
                _logger.Warn("Max occurrences reached. Deactivating trigger.");
                this.DeactivateTrigger("Max occurrences reached.");

                base.RaiseOnFlexiSphereTriggerCompleted(_context);
                return;
            }

            base.IncrementCounter(); // thread-safe for Counter
            base.RaiseOnFlexiSphereTriggered(_context);

            this.ScheduleNextExecution();
        }
        catch (Exception ex)
        {
            this.RaiseOnFlexiSphereTriggerFaulted(_context, ex);
        }
        finally
        {
            _semaphore!.Release();
        }
    }

    public DateTime? GetNextOccurrence()
    {
        _cronExpression.ThrowExceptionIfNull($"{nameof(_cronExpression)} cannot be null!");

        return _cronExpression!.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
    }

    private void ScheduleNextExecution()
    {
        _logger.Info("Scheduling next execution of the trigger");

        _cronExpression.ThrowExceptionIfNull($"{nameof(_cronExpression)} cannot be null!");
        _timer.ThrowExceptionIfNull($"{nameof(_timer)} cannot be null!");

        var nextOccurrence = _cronExpression!.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
        if (nextOccurrence.HasValue)
        {
            if (nextOccurrence.Value < DateTime.UtcNow)
            {
                _logger.Warn("Next occurrence is in the past. Scheduling next occurrence in 2 minutes.");
                nextOccurrence = _cronExpression.GetNextOccurrence(nextOccurrence.Value.AddMinutes(2));
            }

            var delay = nextOccurrence!.Value - DateTime.UtcNow;
            _timer!.Change(delay, Timeout.InfiniteTimeSpan);

            _logger.Info("Next occurrence scheduled at: {0} utc", nextOccurrence.Value);
            return;
        }

        var exp = new FlexiSphereException("Next occurrence is null. Deactivating trigger.");
        _logger.Error(exp);
        throw exp;
    }
}
