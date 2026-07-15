using Avalonia;
using System;
using System.IO;
using System.Linq;

namespace SyncraRPC;

class Program
{
    [STAThread]
    public static void Main(string[] args) {
        // CheckEula(); ???
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
