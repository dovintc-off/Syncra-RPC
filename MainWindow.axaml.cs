// Copyright (C) 2026 Dovintc
// This file is part of Syncra RPC
// Licensed under AGPL-3.0 with No-Misattribution Addendum.
// See LICENSE file for details.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
namespace SyncraRPC;

public partial class MainWindow : Window
{
    private readonly ManagerSyncraRpc _rpcManager = new();
    private readonly Config config;

    public MainWindow()
    {
        InitializeComponent();
        config = new();
    }

    public void OnMenuSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var menuListBox = this.FindControl<ListBox>("MenuListBox");
        var contentCarousel = this.FindControl<Carousel>("ContentCarousel");

        if (menuListBox != null && contentCarousel != null)
        {
            contentCarousel.SelectedIndex = menuListBox.SelectedIndex;
        }
    }

    public async void onActivateSyncraRPC(object? sender, RoutedEventArgs e)
    {
        var rpcButton = this.FindControl<Button>("RpcActivationButton");
        if (rpcButton == null) return;

        if (!_rpcManager.IsActive)
        {
            _rpcManager.Start();
            rpcButton.Content = "Выключить SyncraRPC";
            rpcButton.Background = Avalonia.Media.Brush.Parse("#4e5491"); 
        }
        else
        {
            await _rpcManager.Stop();
            rpcButton.Content = "Включить SyncraRPC";
            rpcButton.Background = Avalonia.Media.Brush.Parse("#5865F2");
        }
    }


    public void OnCloseClick(object? sender, RoutedEventArgs e) => this.Close();
    
    public void OnMinimizeClick(object? sender, RoutedEventArgs e) => 
        this.WindowState = WindowState.Minimized;

    public void OnCloseUI(object? sender, RoutedEventArgs e)
    {
        if (config == null && config.GetStandardConfig("AgreedWithHideUIButton")?.GetType() != typeof(string)) {
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
        await _rpcManager.Stop();
        base.OnClosed(e);
        Console.WriteLine("[RPC] Соединение закрыто");
    }
}
