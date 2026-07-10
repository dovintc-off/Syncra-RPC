using System;
using Avalonia.Markup.Xaml;
using SyncraRPC.Localization;

namespace SyncraRPC.LocalizationMarkupExtension;

public class LocExtension : MarkupExtension {
    public string? Key {get; set;}
    public LocExtension(){}

    public LocExtension(string key) {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        if (string.IsNullOrEmpty(Key)) return Key ?? string.Empty;
        return LocalizationManager.Instance.GetString(Key);
    }
}