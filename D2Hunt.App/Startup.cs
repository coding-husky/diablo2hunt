using D2Hunt.App.Abstraction.Data;
using D2Hunt.App.Data.Providers;
using Microsoft.Extensions.DependencyInjection;
using D2Hunt.App.Abstraction.Infrastructure;
using D2Hunt.App.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace D2Hunt.App;

internal static class Startup
{
    public static IServiceCollection Configure(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<IConnectionsProvider, NetstatConnectionsProvider>();
        services.AddTransient<IConnectionScanner, ConnectionScanner>();
        services.AddConfiguration();;

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var appsettingsStream = Assembly.GetEntryAssembly()?.GetManifestResourceStream("D2Hunt.App.appsettings.json");

        if (appsettingsStream != null)
        {
            services.AddSingleton<IConfiguration>(s => 
                new ConfigurationBuilder()
                    .AddJsonStream(appsettingsStream)
                    .Build());
        }
        
        return services;
    }
        

    public static void ConfigureLogging(this IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetService<IConfiguration>();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}