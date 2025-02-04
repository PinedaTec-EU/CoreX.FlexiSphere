using System.Diagnostics;
using NLog;

namespace FlexiSphere;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class FlexiSphere : IFlexiSphere
{
    public Ulid Id { get; } = Ulid.NewUlid();

    public event FlexiSphereHandler? OnCompleted;
    public event FlexiSphereHandler? OnTriggered;
    public event FlexiSphereHandler? OnCanceled;

    public event FlexiSphereExceptionHandler<Exception>? OnFaulted;

    public event FlexiSphereHandler? OnBeforeJobExecuted;
    public event FlexiSphereHandler? OnAfterJobExecuted;

    public IReadOnlyCollection<IFlexiSphereTrigger> Triggers => _triggers;
    private List<IFlexiSphereTrigger> _triggers = new();

    public IReadOnlyCollection<IFlexiSphereJob> Jobs => _jobs;
    private List<IFlexiSphereJob> _jobs = new();

    public int Counter { get; private set; }
    public DateTime? LastTriggered { get; private set; }

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private CancellationToken _cancellationToken = default;

    public void AddJob(IFlexiSphereJob job)
    {
        try
        {
            _logger.Info("Adding job to FlexiSphere... [{0}]", job.JobName);
            job.ThrowArgumentExceptionIfNull(nameof(job));
            _jobs.Add(job);

            job.OnFaulted += (sender, context, exception) => this.OnFlexiSphereComponentException(sender, context, exception);
        }
        catch (Exception ex)
        {
            var nexp = new FlexiSphereException("An error occurred while adding a job to the FlexiSphere", ex);
            _logger.Error(nexp, "An error occurred while adding a job to the FlexiSphere");
            throw nexp;
        }
    }

    public void AddTrigger(IFlexiSphereTrigger trigger)
    {
        try
        {
            _logger.Info("Adding trigger to FlexiSphere... [{0}]", trigger.TriggerName);
            trigger.ThrowArgumentExceptionIfNull(nameof(trigger));
            _triggers.Add(trigger);

            trigger.OnFaulted += (sender, context, exception) => this.OnFlexiSphereComponentException(sender, context, exception);
            trigger.OnCanceled += (sender, context) => this.OnFlexiSphereTriggerCanceled(sender, context);
            trigger.OnCompleted += (sender, context) => this.OnFlexiSphereTriggerCompleted(sender, context);
            trigger.OnTriggered += (sender, context) => this.OnFlexiSphereTriggered(sender, context);
        }
        catch (Exception ex)
        {
            var nexp = new FlexiSphereException("An error occurred while adding a trigger to the FlexiSphere", ex);
            _logger.Error(nexp, "An error occurred while adding a trigger to the FlexiSphere");
            throw nexp;
        }
    }

    public async Task StartAsync(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        _logger.Info("Starting FlexiSphere...");
        this.Triggers.Any().ThrowExceptionIfFalse<FlexiSphereException>("No triggers have been added to the FlexiSphere!");
        this.Jobs.Any().ThrowExceptionIfFalse<FlexiSphereException>("No jobs have been added to the FlexiSphere!");

        _cancellationToken = cancellationToken;

        _triggers.ForEach(trigger =>
        {
            _logger.Info("Activating trigger [{0}]...", trigger.TriggerName);
            trigger.ActivateTrigger(context, cancellationToken);
        });

        await Task.CompletedTask;
    }

    public async Task StopAsync(string issue, IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        _logger.Info("Stopping FlexiSphere...");
        _triggers.ForEach(trigger =>
        {
            _logger.Info("Deactivating trigger [{0}]...", trigger.TriggerName);
            trigger.DeactivateTrigger(issue, context);
        });

        await Task.CompletedTask;
    }

    private void OnFlexiSphereComponentException(IFlexiSphereComponent sender, IFlexiSphereContext? context, Exception exception)
    {
        _logger.Error(exception, $"An error occurred while executing FlexiSphere [{sender.GetType().Name}]");
        this.RaiseOnFaulted(sender, context, exception);
    }

    private void OnFlexiSphereTriggerCanceled(IFlexiSphereTrigger sender, IFlexiSphereContext? context)
    {
        _logger.Info("FlexiSphere trigger has been canceled [{0}].", sender.TriggerName);
        this.RaiseOnCanceled(sender, context);
    }

    private void OnFlexiSphereTriggerCompleted(IFlexiSphereTrigger sender, IFlexiSphereContext? context)
    {
        _logger.Info("FlexiSphere trigger has completed [{0}].", sender.TriggerName);
        this.RaiseOnCompleted(sender, context);
    }

    private void OnFlexiSphereTriggered(IFlexiSphereTrigger sender, IFlexiSphereContext? context)
    {
        try
        {
            this.LastTriggered = DateTime.UtcNow;

            _logger.Info("FlexiSphere trigger has been triggered [{0}].", sender.TriggerName);
            this.RaiseOnTriggered(sender, context);

            var jobs = _jobs.Where(f => f.IsEnabled).ToList();

            _logger.Debug("Jobs to execute: {0}", jobs.Count);
            jobs.ForEach(async job =>
            {
                _logger.Trace("Invoking job execution [{0}].", job.JobName);

                this.OnBeforeJobExecuted?.Invoke(job, context);

                await job.ExecuteAsync(context, _cancellationToken);

                this.OnAfterJobExecuted?.Invoke(job, context);
            });

            this.Counter++;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while raising the OnTriggered event");
            this.OnFlexiSphereComponentException(this, context, ex);
        }
    }

    private void RaiseOnTriggered(IFlexiSphereTrigger sender, IFlexiSphereContext? context)
    {
        try
        {
            this.OnTriggered?.Invoke(sender, context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while raising the OnTriggered event");
            this.OnFlexiSphereComponentException(this, context, ex);
        }
    }

    private void RaiseOnCompleted(IFlexiSphereTrigger sender, IFlexiSphereContext? context)
    {
        try
        {
            this.OnCompleted?.Invoke(sender, context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while raising the OnCompleted event");
            this.OnFlexiSphereComponentException(this, context, ex);
        }
    }

    private void RaiseOnCanceled(IFlexiSphereTrigger sender, IFlexiSphereContext? context)
    {
        try
        {
            this.OnCanceled?.Invoke(sender, context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while raising the OnCanceled event");
            this.OnFlexiSphereComponentException(this, context, ex);
        }
    }

    private void RaiseOnFaulted(IFlexiSphereComponent sender, IFlexiSphereContext? context, Exception exception)
    {
        try
        {
            this.OnFaulted?.Invoke(sender, context, exception);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while raising the OnFaulted event");
        }
    }

    private string GetDebuggerDisplay() =>
        $"{this.Id}: T:{this.Triggers.Count} J:{this.Jobs.Count}";
}
