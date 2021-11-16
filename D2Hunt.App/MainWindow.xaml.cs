using System.Diagnostics;
using System.Windows;
using D2Hunt.App.Abstraction.Infrastructure;
using D2Hunt.App.Data.Models;
using D2Hunt.App.Data.ViewModels;
using D2Hunt.App.Infrastructure.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;

namespace D2Hunt.App;

public partial class MainWindow : Window
{
    private readonly IConnectionScanner connectionScanner;
    private MainWindowViewModel model;

    public MainWindow(IConnectionScanner connectionScanner)
    {
        this.connectionScanner = connectionScanner;
        model = new MainWindowViewModel();
        this.DataContext = model;
        this.Title = $"{App.Name} v{GetType().Assembly.GetName().Version?.ToString(2)}";
        InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        this.connectionScanner.BeginScan(OnNewGameFound, OnRegionChange);
        Task.Run(() =>
        {
            while (true)
            {
                var lastGame = model.GamesHistory.LastOrDefault();
                if (lastGame != null)
                {
                    var interval = (int)(DateTime.Now - lastGame.CreatedAt).TotalSeconds;
                    Dispatcher.Invoke(() => model.GamesHistory.Last().Delay = interval >= 60 ? 0 : 60 - interval);
                }

                Thread.Sleep(1000);
            }
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        this.connectionScanner.EndScan();
    }

    private Task OnNewGameFound(string address, DateTime createdAt)
    {
        Dispatcher.Invoke(() =>
        {
            foreach (var gameInfo in model.GamesHistory.Where(x => x.Delay > 0))
            {
                gameInfo.Delay = 0;
            }

            var newGame = new GameInfo
            {
                Id = model.GamesHistory.Count + 1,
                Address = address,
                CreatedAt = createdAt,
                IsHot = CheckIfHotIp(model.HotIp, address)
            };

            if (newGame.IsHot)
            {
                Log.ForContext<MainWindow>().LogInfo("Found game with hot IP: {HotIP}@{Region}", newGame.Address, model.Region);
                new ToastContentBuilder()
                    .AddText("You found hot IP game!")
                    .AddText($"IP: {newGame.Address}")
                    .Show();
            }

            model.GamesHistory.Add(newGame);
            ListView.ScrollIntoView(model.GamesHistory.Last());
        });

        return Task.CompletedTask;
    }

    private Task OnRegionChange(string region)
    {
        Trace.WriteLine($"New region: {region}");
        model.Region = region;
        return Task.CompletedTask;
    }

    private static bool CheckIfHotIp(string hotIp, string address) =>
        (hotIp ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(StringHelper.IsValidIpAddress)
            .Any(address.Equals);


    private void ClearListClickHandle(object sender, RoutedEventArgs e)
    {
        model.GamesHistory.Clear();
    }
}