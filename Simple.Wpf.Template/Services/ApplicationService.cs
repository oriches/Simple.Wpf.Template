using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using NLog;
using NLog.Targets;

namespace Simple.Wpf.Template.Services;

[UsedImplicitly]
public sealed class ApplicationService : IApplicationService, IRegisteredService
{
    private string _logFolder;

    public string LogFolder
    {
        get
        {
            if (!string.IsNullOrEmpty(_logFolder)) return _logFolder;

            _logFolder = GetLogFolder();
            return _logFolder;
        }
    }

    public void CopyToClipboard(string text) => Clipboard.SetText(text);

    public void Exit() => Application.Current.Shutdown();

    public void Restart()
    {
        var path = Application.ResourceAssembly.Location;
        if (path.EndsWith(".exe"))
        {
            Process.Start(path);
        }
        else if (path.EndsWith(".dll"))
        {
            var lastIndex = path.LastIndexOf(".dll", StringComparison.Ordinal);
            path = path.Remove(lastIndex) + ".exe";

            Process.Start(path);
        }

        Application.Current.Shutdown();
    }

    public void OpenFolder(string folder) => Process.Start("explorer.exe", folder);

    private static string GetLogFolder()
    {
        var fileTarget = LogManager.Configuration.AllTargets
            .OfType<FileTarget>()
            .SingleOrDefault();

        var fileName = fileTarget?.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now });
        return Path.GetDirectoryName(fileName);
    }
}