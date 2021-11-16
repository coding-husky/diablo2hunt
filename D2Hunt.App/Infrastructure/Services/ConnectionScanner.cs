using D2Hunt.App.Abstraction.Data;
using D2Hunt.App.Abstraction.Infrastructure;
using D2Hunt.App.Infrastructure.Helpers;

namespace D2Hunt.App.Infrastructure.Services;

public class ConnectionScanner : IConnectionScanner
{
    private readonly IConnectionsProvider connectionsProvider;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly CancellationToken cancellationToken;

    private readonly IEnumerable<string> SystemServers = new[] { "127.0.0.1", "24.105.29.76", "34.117.122.6" };
    private readonly IEnumerable<string> EuropeServers = new[] { "37.244.28.80", "37.244.28.180", "37.244.54.10" };
    private readonly IEnumerable<string> NorthAmericaServers = new[] { "137.221.106.88", "137.221.106.188", "137.221.105.152" };
    private readonly IEnumerable<string> AsiaServers = new[] { "117.52.35.45", "117.52.35.79", "117.52.35.179" };

    public ConnectionScanner(IConnectionsProvider connectionsProvider)
    {
        this.connectionsProvider = connectionsProvider;

        this.cancellationTokenSource = new CancellationTokenSource();
        this.cancellationToken = cancellationTokenSource.Token;
    }

    public Task BeginScan(Func<string, DateTime, Task> onNewGameFound, Func<string, Task> onRegionChange, int scanInterval = 1000) =>
        Task.Run(async () =>
        {
            var lastGameAddress = string.Empty;
            var lastRegion = string.Empty;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var connections = await connectionsProvider.List()
                    .Where(x => !SystemServers.Contains(x.RemoteIp))
                    .Select(x => (Address: x.RemoteIp, Region: FindRegion(x.RemoteIp)))
                    .ToListAsync(cancellationToken);

                if (!connections.Any()) continue;
                    
                var currentRegion = connections
                    .GroupBy(x => x.Region)
                    .OrderByDescending(x => x.Count())
                    .FirstOrDefault()?.Key ?? string.Empty;

                if (!string.Equals(currentRegion, lastRegion))
                {
                    Log.ForContext<ConnectionScanner>().LogDebug("Found new region: {Region}", currentRegion);
                    await onRegionChange(currentRegion);
                    lastRegion = currentRegion;
                }

                var currentGameAddresses = connections.Where(x => string.IsNullOrEmpty(x.Region)).Select(x => x.Address).ToList();
                foreach(var currentGameAddress in currentGameAddresses)
                {
                    if (!string.Equals(currentGameAddress, lastGameAddress) && !string.IsNullOrWhiteSpace(currentGameAddress))
                    {
                        Log.ForContext<ConnectionScanner>().LogDebug("Found new game: {GameAddress}", currentGameAddress);
                        await onNewGameFound(currentGameAddress, DateTime.Now);
                        lastGameAddress = currentGameAddress;
                    }
                }

                Thread.Sleep(scanInterval);
            }
        }, cancellationToken);


    public void EndScan() => this.cancellationTokenSource.Cancel();

    private string FindRegion(string address) =>
        address switch
        {
            var x when EuropeServers.Contains(x) => "EU",
            var x when NorthAmericaServers.Contains(x) => "NA",
            var x when AsiaServers.Contains(x) => "Asia",
            _ => string.Empty
        };
}
