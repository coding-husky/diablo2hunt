using System.Net.Http;
using System.Windows;
using D2Hunt.App.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace D2Hunt.App;

public partial class App : Application
{
    public static string Name { get; } = "D2R IP Hunt";
    private readonly ServiceProvider serviceProvider;

    public App()
    {
        this.serviceProvider = new ServiceCollection().Configure().BuildServiceProvider();
        this.serviceProvider.ConfigureLogging();

        this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Log.ForContext<App>().LogInfo("Application started");
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
                    MessageBox.Show("A new version is available. Do you want to download it?", Name, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                Log.ForContext<App>().LogError(e, "Unable to read latest version from server!");
            }
        });

    private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Log.ForContext<App>().LogError(e.Exception, "An unhandled exception occured {Message}", e.Exception.Message);
        e.Handled = true;
    }

}