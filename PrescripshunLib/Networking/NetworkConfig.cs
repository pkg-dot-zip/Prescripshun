using System.Net;

namespace PrescripshunLib.Networking;

/// <summary>
/// Contains all configurable values for the networking system. Think of things like port numbers.
/// </summary>
public static class NetworkConfig
{
    public const int Port = 27000;
    public const bool UseIPv6 = true; // IPv4 not supported by current system.

    public static IPAddress LocalIpAddress => UseIPv6 ? IPAddress.IPv6Loopback : IPAddress.Loopback;
    public static IPAddress AnyIpAddress => UseIPv6 ? IPAddress.IPv6Any : IPAddress.Any;
}