using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SyncraRPC.Localization;

namespace SyncraRPC;

public partial class App : Application {
    public Config? config;
    public override void Initialize() {
        config = new();
        LocalizationManager.Instance.SetLanguage(config.GetStandardConfig("Language")?.ToString()??"EN");
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted(){
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}