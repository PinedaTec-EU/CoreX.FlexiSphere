# FlexiSphereFactory

Factory to help you to build the component [FlexiSphere](./FlexiSphere.md) with your triggers, and jobs

## Usage samples

### Related factories

- [FlexiSphereTriggerFactory](./FlexiSphereTriggerFactory.md)
- [FlexiSphereJobFactory](./FlexiSphereJobFactory.md)

```C#
using FlexiSphere;

var fsFactory = FlexiSphereFactory.Create();

fsFactory
    .AddTrigger(fstFactory =>
    {
        fstFactory
            .WithTriggerName("SampleWorkerTrigger", "SampleWorker")
            .FireTriggerOnStart(true)
            .StartOn("0 */20 * * * *");
    })
    .AddJob(fsjFactory =>
    {
        fsjFactory
            .WithJobName("SampleWorkerJob", "SampleWorker")
            .SetRateLimiter(TimeSpan.FromMinutes(5), 1)
            .SetJobAction(async context =>
            {
                // Do something really important here!
                await Task.Delay(500);
            });
    });

var fSphere = fsFactory.Build();
fSphere.StartAsync();
```

## Additional considerations

FlexiSphereFactory don't allow you to build a [FlexiSphere](./flexisphere.md) without triggers or without jobs. Atleast one of every one is required to build it.
