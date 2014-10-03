Simple.Wpf.Template
===================

This is a template for a WPF solution based on an MMVM approach using IoC, logging, and asynchronous invocations. All external thrid party libraries are resolved using NuGet.


The idea is to prevent me from re-creating the scaffolding\infrastructure for any future WPF apps, the following are included:

**MVVM** - _implmented as ViewModel first approach using strongly typed XAML DataTemplates,_

**IoC** - _implmented using Autofac, all services and key (major) ViewModels are resolved via the IoC container,_

**Logging** - _implemented using NLog, writes to file currently in the 'C:\temp' directory,_

**Async Support** - _implemented using Reactive Extensions,_

**Heartbeat** - _produces a regular heart beat for the application (currently writes to the log),_

**Idling** - _a service producing notifications when the dispatcher enters idle state,_

**Diagnostics** - _a service exposing the CPU and memory (managed &amp; unmanaged) consumptions,_

**Duration** - _a service for measuring the time for a block of code (Debug mode only),_

**Gestures** - _a service for changing the system gestures (mouse) during the application, designed to be used with MVVM,_
