using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace D2Hunt.App;

public partial class App : Application
{
    public static string Name { get; } = "D2R IP Hunt";
    private readonly ServiceProvider serviceProvider;

    public App()
    {
        this.serviceProvider = new ServiceCollection().Configure().BuildServiceProvider();
        this.serviceProvider.ConfigureLogging();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Log.ForContext<App>().Information("Application started");
        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow?.Show();
        CheckNewVersion();
    }

    private Task CheckNewVersion() =>
        Task.Run(async () =>
        {
            var client = new HttpClient();
            try
            {
                var response = await client.GetAsync("https://coding-husky.github.io/diablo2hunt.version");
                if (!response.IsSuccessStatusCode) return;
                
                var latestVersion = new Version(await response.Content.ReadAsStringAsync()) ?? new Version(0, 0);
                var currentVersion = GetType().Assembly.GetName().Version ?? new Version(0, 0);

                if (currentVersion < latestVersion)
                {
                    var dialogResult = MessageBox.Show("A new version is available. Do you want to download it?", Name, MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://github.com/coding-husky/diablo2hunt",
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Log.ForContext<App>().Error(e, "Unable to read latest version from server!");
            }
        });
}