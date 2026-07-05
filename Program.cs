using Avalonia;
using System;
using System.IO;
using System.Linq;

namespace SyncraRPC;

class Program
{
    [STAThread]
    public static void Main(string[] args) {
        CheckEula();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    #pragma warning disable CA1416
    static void CheckEula()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EULA.txt");
        bool accepted = false;

        try{
            if (File.Exists(path)) accepted = File.ReadAllLines(path).Any(l => l.Trim() == "EULA=true");
        }
        catch { }

        // if (!accepted) {
        //     DialogResult result = MessageBox.Show(
        //         "Нажимая на кнопку 'ОК' вы соглашаетесь с условиями EULA программы SyncraRPC",
        //         "SyncraRPC - Лицензия", 
        //         MessageBoxButtons.OKCancel, 
        //         MessageBoxIcon.Information
        //     );

        //     if (result == DialogResult.OK) {
        //         try {
        //             File.WriteAllText(path, "EULA=true");
        //         } catch (Exception ex) {
        //             MessageBox.Show($"Не удалось сохранить согласие: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //             Environment.Exit(0);
        //         }
        //     } else {
        //         Environment.Exit(0);
        //     }
        // }
    }
    #pragma warning restore CA1416

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
