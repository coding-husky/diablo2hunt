using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using D2Hunt.App.Abstraction.Data;
using D2Hunt.App.Data.Models;
using Serilog;

namespace D2Hunt.App.Data.Providers;

public class NetstatConnectionsProvider : IConnectionsProvider
{
    public IAsyncEnumerable<ConnectionInfo> List()
    {
        var test = Process.GetProcesses();

        var processId = GetProcessId("d2r");
        return GetConnections().Where(x => x.ProcessId == processId);
    }

    private static async IAsyncEnumerable<ConnectionInfo> GetConnections()
    {
        var netstatProcess = new Process
        {
            StartInfo = new ProcessStartInfo("netstat")
            {
                Arguments = "-no -p TCP",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        netstatProcess.Start();

        using var streamReader = netstatProcess.StandardOutput;
        while (!streamReader.EndOfStream)
        {
            var line = (await streamReader.ReadLineAsync()) ?? string.Empty;

            var connectionInfo = ProcessLine(line);
            if (connectionInfo.IsValid)
            {
                yield return connectionInfo;
            }
        }
    }

    private static ConnectionInfo ProcessLine(string line)
    {
        var data = Regex.Matches(line, @"\s+tcp\s+(.*):(\d+)\s+(.*):(\d+)\s+established\s+(\d+)", RegexOptions.IgnoreCase);

        if (!data.Any()) return new();
        
        try
        {
            return new()
            {
                LocalIp = data[0].Groups[1].Value,
                LocalPort = int.Parse(data[0].Groups[2].Value),
                RemoteIp = data[0].Groups[3].Value,
                RemotePort = int.Parse(data[0].Groups[4].Value),
                ProcessId = int.Parse(data[0].Groups[5].Value)
            };
        }
        catch (Exception e)
        {
            Log.ForContext<NetstatConnectionsProvider>().Error(e, "Unable to parse line: {Line}", line);
        }

        return new();
    }

    private static int GetProcessId(string processName) => Process.GetProcessesByName(processName).FirstOrDefault()?.Id ?? 0;
}