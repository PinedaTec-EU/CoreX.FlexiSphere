using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NLog;

using System.Threading.RateLimiting;

namespace FlexiSphere.jobs;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class FlexiSphereJob : IFlexiSphereJob
{
    public event FlexiSphereJobExceptionHandler<Exception>? OnFaulted;

    public Ulid Id { get; } = Ulid.NewUlid();
    public int MaxConcurrents { get; set; } = 1;
    public string JobName { get; set; } = string.Empty;
    public string? JobGroup { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;
    public bool IsRateLimiterEnabled { get; set; } = false;

    private Func<IFlexiSphereContext?, Task> _jobAction = null!;

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private RateLimiter? _rateLimiter = null;

    public void ConfigureJob(string jobName, string? jobGroup, int maxConcurrent = 1, TimeSpan? rateLimiter = null) =>
        this.ConfigureJob(jobName, jobGroup, _ => { return Task.CompletedTask; }, maxConcurrent, rateLimiter);

    public void ConfigureJob(string jobName, string? jobGroup, Func<IFlexiSphereContext?, Task> jobAction, int maxConcurrent = 1, TimeSpan? rateLimiter = null)
    {
        try
        {
            this.JobName = jobName;
            this.JobGroup = jobGroup;
            this.MaxConcurrents = maxConcurrent;
            _jobAction = jobAction;

            if (rateLimiter.HasValue)
            {
                _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
                {
                    TokenLimit = maxConcurrent,
                    TokensPerPeriod = maxConcurrent,
                    ReplenishmentPeriod = rateLimiter.Value
                });

                this.IsRateLimiterEnabled = true;
            }

            jobName.ThrowArgumentExceptionIfNullOrEmpty(nameof(jobName));
            jobAction.ThrowArgumentExceptionIfNull(nameof(jobAction));
            (maxConcurrent < 1).ThrowArgumentExceptionIfTrue(nameof(maxConcurrent), $"{nameof(maxConcurrent)} must be greater than 0!");

            _logger.Info($"[{this.JobGroup}:{this.JobName}] Job configured successfully.");
        }
        catch (Exception ex)
        {
            var nexp = new FlexiSphereException($"[{this.JobGroup}:{this.JobName}] An error occurred while configuring the job", ex);
            _logger.Error(nexp, "An error occurred while configuring the job.");
            this.RaiseOnJobFaulted(nexp);

            throw nexp;
        }
    }

    public async Task ExecuteAsync(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (this.IsRateLimiterEnabled)
            {
                var lease = _rateLimiter!.AttemptAcquire(1)!;

                if (lease.IsAcquired)
                {
                    await _jobAction(context);
                    _logger.Info($"{this} Job executed successfully.");
                }
                else
                {
                    _logger.Warn($"{this} Job execution skipped due to rate limiting.");
                }

                return;
            }

            await _jobAction(context);
            _logger.Info($"{this} Job executed successfully.");
        }
        catch (Exception ex)
        {
            this.RaiseOnJobFaulted(ex);
        }
    }

    protected void RaiseOnJobFaulted(Exception exception, IFlexiSphereContext? context = null)
    {
        try
        {
            this.OnFaulted?.Invoke(this, context, exception);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"[{this}] An error occurred while invoking the OnFaulted event.");
        }
    }

    public override string ToString() =>
        this.JobGroup is not null ? $"[{this.Id}:{this.JobGroup}|{this.JobName}]" : $"[{this.Id}:{this.JobName}]";

    private string GetDebuggerDisplay() =>
        this.JobGroup is not null ? this.JobGroup + ":" + this.JobName : this.JobName;
}
