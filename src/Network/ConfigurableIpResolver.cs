// <copyright file="ConfigurableIpResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;
using Microsoft.Extensions.Logging;

/// <summary>
/// A resolver, which can be re-configured during runtime.
/// </summary>
public class ConfigurableIpResolver : IIpAddressResolver
{
    private readonly ILoggerFactory _loggerFactory;

    private IpResolverType _resolverType;
    private string? _parameter;
    private IIpAddressResolver? _effectiveResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurableIpResolver"/> class.
    /// </summary>
    /// <param name="resolverType">Type of the resolver.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <exception cref="System.ArgumentException">When using a custom resolver type, a parameter with an IP or host name is required. - parameter</exception>
    public ConfigurableIpResolver(IpResolverType resolverType, string? parameter, ILoggerFactory loggerFactory)
    {
        this.Configure(resolverType, parameter);
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Occurs when the configuration has been changed.
    /// </summary>
    public event EventHandler? ConfigurationChanged;

    /// <inheritdoc />
    public async ValueTask<IPAddress> ResolveIPv4Async()
    {
        var resolver = this._effectiveResolver ??= this.CreateResolver();
        return await resolver.ResolveIPv4Async().ConfigureAwait(false);
    }

    /// <summary>
    /// Configures this resolver.
    /// </summary>
    /// <param name="resolverType">Type of the resolver.</param>
    /// <param name="parameter">The parameter.</param>
    public void Configure(IpResolverType resolverType, string? parameter)
    {
        if (resolverType == IpResolverType.Custom && string.IsNullOrWhiteSpace(parameter))
        {
            throw new ArgumentException("When using a custom resolver type, a parameter with an IP or host name is required.", nameof(parameter));
        }

        this._effectiveResolver = null;
        this._resolverType = resolverType;
        this._parameter = parameter;

        this.ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }

    private IIpAddressResolver CreateResolver()
    {
        switch (this._resolverType)
        {
            case IpResolverType.Local:
                return new LocalIpResolver();
            case IpResolverType.Loopback:
                return new LoopbackIpResolver();
            case IpResolverType.Custom when IPAddress.TryParse(this._parameter, out var ip):
                return new CustomIpResolver(ip);
            case IpResolverType.Custom:
                return new HostNameIpResolver(this._parameter!);

            case IpResolverType.Auto when Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development":
                // During development, we use a loopback IP (127.127.127.127).
                return new LoopbackIpResolver();

            // In production our systems should expose itself with their corresponding public IPs.
            case IpResolverType.Auto:
            case IpResolverType.Public:
            default:
                return new PublicIpResolver(_loggerFactory.CreateLogger<PublicIpResolver>());
        }
    }
}