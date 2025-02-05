# FlexiSphere

This component is similar to the public component Quartz, We will discuss the differences between Quartz and FlexiSphere later
I used to design and build these kinds of tools/components to learn how to create them and to tailor the behaviors to more specific needs.

So, this component left aside:

- the persistence logic, but if you want to extend this behavior, is easy to accomplish, using OnBeforeJob, OnAfterjob
- The logging configuration, We use ILogger (NLog), and you can configure it to change the trace level, or the logging destination, GrayLog, console, file, etc

Checks project documents [README.md](https://github.com/PinedaTec-EU/FlexiSphere/blob/main/README.md)
