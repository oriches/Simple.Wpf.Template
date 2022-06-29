Simple.Wpf.Template
===================

[![Build status](https://ci.appveyor.com/api/projects/status/bo9i0a1bajgne80p/branch/master?svg=true)](https://ci.appveyor.com/project/oriches/simple-wpf-template/branch/master)

This is 'my' canonical example for a WPF solution based on an MMVM approach using IoC, logging, and asynchronous invocations. All external 

Built with .NET 6.0 and all the lovely syntatic-sugar available!

**Thrid party libraries are resolved using NuGet.**


The idea is to prevent me from re-creating the scaffolding\infrastructure for any future WPF apps, the following are included:

**MVVM** - _implmented as ViewModel first approach using strongly typed XAML DataTemplates,_

**IoC** - _implmented using Autofac, all services and key (major) ViewModels are resolved via the IoC container,_

**Logging** - _implemented using NLog, writes to file currently in the '%TEMP_FOLDER%\Simple.Wpf.Template' directory,_

**Async Support** - _implemented using Reactive Extensions & TPL (async / await),_

**Modules & Module Loader** - _allows clean setup & configuration, see App.cs,_

**Duration** - _a service for measuring the time for a block of code (Debug mode only),_

**Gestures** - _a service for changing the system gestures (mouse) during the application, designed to be used with MVVM,_

**Event Aggregator** - _a service for publishing / subscribing to application events using Rx syntax,_

**Notifications** - _a service for showing Windows toast notifications,_
