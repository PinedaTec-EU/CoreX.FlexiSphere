# FlexiSphereTriggerFactory

This factory will help you to build triggers, at this moment only ScheduledTriggers or EventTriggers,
THis factory if you don't provided a custom [IFlexiSphereEventTrigger](./IFlexiSphereEventTrigger.md) or a custom [IFlexiSphereScheduledTrigger](./IFlexiSphereScheduledTrigger.md), then the factory uses [FlexiSphereEventTrigger](./FlexiSphereEventTrigger.md) and [FlexiSphereScheduledTrigger](./FlexiSphereScheduledTrigger.md)

## Usage samples

### Related factories

- [FlexiSphereFactory](./FlexiSphereFactory.md)
- [FlexiSphereJobFactory](./FlexiSphereJobFactory.md)

```C#
using FlexiSphere;

var fstFactory = FlexiSphereTriggerFactory.Create();

// Scheduled trigger
fstFactory
    .WithTriggerName("SampleWorkerTrigger", "SampleWorker")
    .SetMaxConcurrent(5)
    .SetMaxOccurences(7);
    .FireTriggerOnStart(true)
    .StartOn("0 */20 * * * *");

var trigger = fstFactory.Build();

trigger.ActivateTrigger(null!);
```

```C#
using FlexiSphere;

var fstFactory = FlexiSphereTriggerFactory.Create();

// Scheduled trigger
fstFactory
    .WithTriggerName("SampleWorkerTrigger", "SampleWorker")
    .SetMaxConcurrent(5)
    .SetMaxOccurences(7);
    .FireTriggerOnStart(true)
    .StartOn("0 */20 * * * *");

var trigger = fstFactory.Build();

trigger.ActivateTrigger(null!);

trigger.OnTriggered += (sender, args) =>
{
    // Do something relevant!
};

trigger.OnFaulted += (sender, context, exception) =>
{
    // Calls mom!
};

trigger.OnCompleted += (sender, context) =>
{
    // wait, what!!??
};
```

###  Aditional considerations

If you set SetMaxOccurrences, you are telling to the trigger that when the trigger reach the number of activations equals to the number configured in the method the trigger would be disabled. And raise a OnTriggerCompleted event.

Thats means, if you create a TimeSphere with a trigger (only this trigger) with that behavior, the FlexiSphere won't warn you about that, and the jobs will never be called again.
