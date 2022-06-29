// <copyright file="IpAddressResolverFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Factory which creates an <see cref="IIpAddressResolver"/> depending on command line arguments.
/// </summary>
public static class IpAddressResolverFactory
{
    private const string ResolveParameterPrefix = "-resolveIP:";
    private const string PublicIpResolve = "-resolveIP:public";
    private const string LocalIpResolve = "-resolveIP:local";
    private const string LoopbackIpResolve = "-resolveIP:loopback";

    /// <summary>
    /// Determines the ip resolver based on command line arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>
    /// The determined resolver.
    /// </returns>
    public static IIpAddressResolver DetermineIpResolver(string[] args, ILoggerFactory loggerFactory)
    {
        var parameter = GetParameter(args);

        switch (parameter)
        {
            case null:
            case { } p when p.StartsWith(PublicIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return new PublicIpResolver(loggerFactory.CreateLogger<PublicIpResolver>());
            case { } p when p.StartsWith(LocalIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return new LocalIpResolver();
            default:
                var ipString = ExtractIpFromParameter(parameter);
                if (IPAddress.TryParse(ipString, out var ip))
                {
                    return new CustomIpResolver(ip);
                }

                throw new ArgumentException($"The ip address '{ipString}' is in the wrong format.");
        }
    }

    /// <summary>
    /// Adds the ip resolver to the collection, depending on the command line arguments.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The <paramref name="serviceCollection"/>.</returns>
    public static IServiceCollection AddIpResolver(this IServiceCollection serviceCollection, params string[] args)
    {
        var parameter = GetParameter(args);
        var environmentVariable = Environment.GetEnvironmentVariable("RESOLVE_IP");
        switch (parameter)
        {
            case null when environmentVariable is { }:
                return serviceCollection.AddIpResolverByEnvironment();
            case null:
            case { } p when p.StartsWith(PublicIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return serviceCollection.AddSingleton<IIpAddressResolver, PublicIpResolver>();
            case { } p when p.StartsWith(LocalIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return serviceCollection.AddSingleton<IIpAddressResolver, LocalIpResolver>();
            case { } p when p.StartsWith(LoopbackIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return serviceCollection.AddSingleton<IIpAddressResolver, LoopbackIpResolver>();
            default:
                var ipString = ExtractIpFromParameter(parameter);
                if (IPAddress.TryParse(ipString, out var ip))
                {
                    return serviceCollection.AddSingleton<IIpAddressResolver>(new CustomIpResolver(ip));
                }

                throw new ArgumentException($"The ip address '{ipString}' is in the wrong format.");
        }
    }

    /// <summary>
    /// Adds a <see cref="IIpAddressResolver"/> depending on the environment.
    /// In a development environment, a <see cref="LoopbackIpResolver"/> is added.
    /// In a production environment, a <see cref="PublicIpResolver"/> is added.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddIpResolverByEnvironment(this IServiceCollection services)
    {
        if (Environment.GetEnvironmentVariable("RESOLVE_IP") is { } resolveIp)
        {
            if (IPAddress.TryParse(resolveIp, out var customIp))
            {
                return services.AddCustomIpResolver(customIp);
            }

            if (resolveIp == "local")
            {
                return services.AddSingleton<IIpAddressResolver, LocalIpResolver>();
            }

            if (resolveIp == "public")
            {
                return services.AddSingleton<IIpAddressResolver, PublicIpResolver>();
            }

            if (resolveIp == "loopback")
            {
                return services.AddSingleton<IIpAddressResolver, LoopbackIpResolver>();
            }
        }

        var isDevelopmentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isDevelopmentEnvironment)
        {
            // During development, we use a loopback IP (127.127.127.127).
            return services.AddSingleton<IIpAddressResolver, LoopbackIpResolver>();
        }

        // In production our systems should expose itself with their corresponding public IPs.
        return services.AddSingleton<IIpAddressResolver, PublicIpResolver>();
    }

    /// <summary>
    /// Adds the <see cref="CustomIpResolver"/> with the specified address.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="address">The address.</param>
    /// <returns>The services.</returns>
    public static IServiceCollection AddCustomIpResolver(this IServiceCollection services, IPAddress address)
    {
        return services.AddSingleton<IIpAddressResolver>(s => new CustomIpResolver(address));
    }

    private static string ExtractIpFromParameter(string parameter) => parameter.Substring(parameter.IndexOf(':') + 1);

    private static string? GetParameter(string[] args) => args.FirstOrDefault(a => a.StartsWith(ResolveParameterPrefix, StringComparison.InvariantCultureIgnoreCase));
}