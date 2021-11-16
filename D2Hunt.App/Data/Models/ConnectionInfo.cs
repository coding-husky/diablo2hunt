using D2Hunt.App.Infrastructure.Helpers;

namespace D2Hunt.App.Data.Models;

public record ConnectionInfo
{
    public string LocalIp { get; set; } = string.Empty;
    public int LocalPort { get; set; } = 0;
    public string RemoteIp { get; set; } = string.Empty;
    public int RemotePort { get; set; } = 0;
    public int ProcessId { get; set; } = 0;

    public bool IsValid => StringHelper.IsValidIpAddress(LocalIp) 
                           && LocalPort > 0 
                           && StringHelper.IsValidIpAddress(RemoteIp) 
                           && RemotePort > 0
                           && ProcessId > 0;

}