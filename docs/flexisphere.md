# FlexiSphere

This component is similar to the public component Quartz, We will discuss the differences between Quartz and FlexiSphere later
I used to design and build these kinds of tools/components to learn how to create them and to tailor the behaviors to more specific needs.

So, this component left aside:

- the persistence logic, but if you want to extend this behavior, is easy to accomplish, using OnBeforeJob, OnAfterjob
- The logging configuration, We use ILogger (NLog), and you can configure it to change the trace level, or the logging destination, GrayLog, console, file, etc

Realted interfaces/components

- [IFlexiSphereTrigger](./IFlexiSphereTrigger.md)
- [IFlexiSphereJob](./IFlexiSphereJob.md)

## Factory

- [FlexiSphereFactory](./FlexiSphereFactory.md)

## Hands-on

This component requires atleast one trigger, and one job to work

```mermaid
flowchart TD
    FS[FlexiSphere] --> FST[Triggers]
    FST --> FS
    FS --> FSJ[Jobs]
```

The triggers are activated by FlexiSphere and when a trigger is triggered raise a notification to FlexiSphere OnTriggered.

When FlexiSphere receives the event, it runs the defined jobs, and executes each job.
FlexiSphere is responsilbe to throw events such as:

- OnTriggered: Raised when the trigger is triggered.
- OnTriggerCompleted: When the trigger has finished, and it is disabled for further executions.
- OnBeforeJob: Raised just before the job action is invoked.
- OnAfterJob: Raised just after the job action is invoked.
- OnCancelled: Raised when is requested the cancellation to the cancellation token source is requested.
- OnFaulted: Raised when any of the components, triggers or jobs throws an exception.

Sequence diagram:

```mermaid
sequenceDiagram
    actor user
    participant FlexiSphere
    participant Trigger
    participant Job

    FlexiSphere <<->>+ Trigger: ActivateTrigger()
    Trigger -->>+ FlexiSphere: OnTriggered()
    FlexiSphere -->> user: OnTriggered()

    FlexiSphere -->> user: OnBeforeJobExecuted()
    FlexiSphere ->>+ Job: ExecuteAsync()
    Job -->>- FlexiSphere: Done
    FlexiSphere -->> user: OnAfterJobExecuted()
```

Class diagram:

```mermaid
classDiagram
    interface IFlexiSphereContext

    class IFlexiSphere {
        +IReadOnlyCollection~IFlexiSphereTrigger~ Triggers
        +IReadOnlyCollection~IFlexiSphereJob~ Jobs
        +int Counter
        +StartAsync()
    }

    class IFlexiSphereTrigger {
        +ActivateTrigger(IFlexiSphereContext)
        +OnFaulted() event
        +OnTriggered() event
        +OnCompleted() event
    }

    class IFlexiSphereJob {
        +ExecuteAsync(IFlexiSphereContext)
        +OnFaulted() event
    }

    IFlexiSphere "1" --> "1..*" IFlexiSphereTrigger
    IFlexiSphere "1" --> "1..*" IFlexiSphereJob
```
