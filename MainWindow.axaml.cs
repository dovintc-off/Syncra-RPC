using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace SyncraRPC;

public partial class MainWindow : Window
{
    private readonly ManagerSyncraRpc _rpcManager = new();

    public MainWindow()
    {
        InitializeComponent();
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

    public void onActivateSyncraRPC(object? sender, RoutedEventArgs e)
    {
        var rpcButton = this.FindControl<Button>("RpcActivationButton");
        if (rpcButton == null) return;

        if (!_rpcManager.IsActive)
        {
            _rpcManager.Start();
            rpcButton.Content = "Выключить SyncraRPC";
            rpcButton.Background = Avalonia.Media.Brush.Parse("#d32f2f"); 
        }
        else
        {
            _rpcManager.Stop();
            rpcButton.Content = "Включить SyncraRPC";
            rpcButton.Background = Avalonia.Media.Brush.Parse("#416188");
        }
    }

    public void OnCloseClick(object? sender, RoutedEventArgs e) => this.Close();
    
    public void OnMinimizeClick(object? sender, RoutedEventArgs e) => 
        this.WindowState = WindowState.Minimized;
    
    public void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e) => 
        this.BeginMoveDrag(e);
        
    protected override void OnClosed(EventArgs e)
    {
        _rpcManager.Stop();
        base.OnClosed(e);
        Console.WriteLine("[RPC] Соединение закрыто");
    }
}
