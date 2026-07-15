// Copyright (C) 2026 Dovintc
// This file is part of Syncra RPC
// Licensed under AGPL-3.0 with No-Misattribution Addendum.
// See LICENSE file for details.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using SyncraRPC.Localization;

namespace SyncraRPC;

public partial class MainWindow : Window
{
    private readonly ManagerSyncraRpc _rpcManager = new();
    public readonly Config config;

    public MainWindow() {
        if (App.Current is App myApp && myApp.config != null) config = myApp.config;
        InitializeComponent();

        if (config != null)
        {
            var rawSomeBtn = config.GetStandardConfig("SomeBtn");
            bool isEnabledByDefault = rawSomeBtn is bool b ? b : true;
            ShowTimestampCheckBox.IsChecked = isEnabledByDefault;

            var rawOutputInfo = config.GetStandardConfig("OutputOfAdditionalInformation")?.ToString();

            if (rawOutputInfo == "CriticalErrorsOnly")
            {
                CriticalErrorsRadio.IsChecked = true;
            } else {
                SendAllRadio.IsChecked = true;
            }
        }
    }


    private void OnShowTimestampChanged(object? sender, RoutedEventArgs e)
    {
        if (config == null || sender is not CheckBox checkBox) return;

        string valueToSave = checkBox.IsChecked == true ? "true" : "false";

        System.Console.WriteLine($"[Config] ShowTimestamp изменен на: {valueToSave}");
    }


    public void OnMenuSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        var menuListBox = this.FindControl<ListBox>("MenuListBox");
        var contentCarousel = this.FindControl<Carousel>("ContentCarousel");

        if (menuListBox != null && contentCarousel != null)
        {
            contentCarousel.SelectedIndex = menuListBox.SelectedIndex;
        }
    }

    public async void onActivateSyncraRPC(object? sender, RoutedEventArgs e) {
        var rpcButton = this.FindControl<Button>("RpcActivationButton");
        if (rpcButton == null) return;

        if (!_rpcManager.IsActive) {
            _rpcManager.Start();
            rpcButton.Content = LocalizationManager.Instance.GetString("main.mainBtn.off");
            rpcButton.Background = Avalonia.Media.Brush.Parse("#4e5491"); 
        } else {
            await _rpcManager.Stop();
            rpcButton.Content = LocalizationManager.Instance.GetString("main.mainBtn.on");
            rpcButton.Background = Avalonia.Media.Brush.Parse("#5865F2");
        }
    }

    public void OnCloseClick(object? sender, RoutedEventArgs e) => this.Close();
    
    public void OnMinimizeClick(object? sender, RoutedEventArgs e) => 
        this.WindowState = WindowState.Minimized;

    public void OnCloseUI(object? sender, RoutedEventArgs e)
    {   
        if (config == null) return;
        if (config.GetStandardConfig("AgreedWithHideUIButton")?.GetType() != typeof(string)) {
            System.Console.WriteLine("[Main] Config пуст или прочитан правильно пора дебажить");
        }        

        if (config.GetStandardConfig("AgreedWithHideUIButton")?.ToString()?.ToLower().Trim() == "false")
        {
            WarningOverlay.IsVisible = true;
            WarningOverlay.ZIndex = 1000;
        }
    }

    public void BtnAcceptHidingTrayOk(object? sender, RoutedEventArgs e)
    {
        WarningOverlay.IsVisible = false;
        WarningOverlay.ZIndex = -1000;
        config.SetStandardConfig(agreedWithHideUIButton: "true");
    }

    public void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e) => 
        this.BeginMoveDrag(e);
        
    protected override async void OnClosed(EventArgs e)
    {
        if (config != null)
        {
            string someBtnState = ShowTimestampCheckBox?.IsChecked == true ? "true" : "false";

            string outputInfoState = "SendAll";
            
            if (CriticalErrorsRadio?.IsChecked == true){
                outputInfoState = "CriticalErrorsOnly";
            }

            config.SetStandardConfig(
                SomeBtn: someBtnState,
                OutputOfAdditionalInformation: outputInfoState
            );

            Console.WriteLine($"[Config] Настройки сохранены. SomeBtn: {someBtnState}, OutputInfo: {outputInfoState}");
        }

        await _rpcManager.Stop();
        base.OnClosed(e);
        Console.WriteLine("[RPC] Соединение закрыто");
    }
}
