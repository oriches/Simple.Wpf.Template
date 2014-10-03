Simple.Wpf.Template
===================

This is a template for a WPF solution based on an MMVM approach using IoC, logging, and asynchronous invocations. All external thrid party libraries are resolved using NuGet.


The idea is to prevent me from re-creating the scaffolding\infrastructure for a WPF application, the following are included:

&nbsp;&nbsp;&nbsp;MVVM - implmented as ViewModel first approach using strongly typed XAML DataTemplates,

&nbsp;&nbsp;&nbsp;IoC - implmented using Autofac, all services and key (major) ViewModels are resolved via the IoC container,

&nbsp;&nbsp;&nbsp;Logging - implemented using NLog, writes to file currently in the 'C:\temp' directory,

&nbsp;&nbsp;&nbsp;Async support - implemented using Reactive Extensions,

&nbsp;&nbsp;&nbsp;Heartbeat - produces a regular heart beat for the application (currently writes to the log),

&nbsp;&nbsp;&nbsp;Idling - a service producing notifications when the dispatcher enters idle state,

&nbsp;&nbsp;&nbsp;Diagnostics - a service exposing the CPU and memory (managed &amp; unmanaged) consumptions,

&nbsp;&nbsp;&nbsp;Duration - a service for measuring the time for a block of code (Debug mode only),

&nbsp;&nbsp;&nbsp;Gestures - a service for changing the system gestures (mouse) during the application, designed to be used with MVVM,
