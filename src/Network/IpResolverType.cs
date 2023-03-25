namespace MUnique.OpenMU.Network;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Defines how the own IP should be resolved and reported to the game client.
/// </summary>
public enum IpResolverType
{
    /// <summary>
    /// The IP is detected automatically by considering the environment.
    /// </summary>
    [Display(Name = "Automatic", Description = "The IP is detected automatically by considering the environment. No parameter required.")]
    Auto,

    /// <summary>
    /// The IP is detected by asking an external service. As a result, we resolve
    /// to the public IP.
    /// </summary>
    [Display(Name = "Public", Description = "The IP is detected by asking an external service (https://www.ipify.org/) about the own IP. No parameter required. Use this, when the server is running on a server and should be reachable from the internet. Firewall is required to be opened for the ports of game, connect and chat servers.")]
    Public,

    /// <summary>
    /// The IP is detected by asking the DNS about the IP of the own host name.
    /// </summary>
    [Display(Name = "Local", Description = "The IP is detected by asking the DNS about the IP of the own host name. The first IP is used. No parameter required. Use this, if you want to use the server within a local LAN, e.g. at home.")]
    Local,

    /// <summary>
    /// The IP is resolved as '127.127.127.127'.
    /// </summary>
    [Display(Name = "Loopback", Description = "The IP is resolved as '127.127.127.127'. No parameter required. Use this, if you want to use the server and client just within your own computer, e.g. during development.")]
    Loopback,

    /// <summary>
    /// The IP or a host name can be set manually.
    /// </summary>
    [Display(Name = "Custom", Description = "The IP or a host name can be set manually. Use this, if the other options don't work for you.")]
    Custom,
}