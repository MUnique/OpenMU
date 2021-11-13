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

        switch (parameter)
        {
            case null:
            case { } p when p.StartsWith(PublicIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return serviceCollection.AddSingleton<IIpAddressResolver, PublicIpResolver>();
            case { } p when p.StartsWith(LocalIpResolve, StringComparison.InvariantCultureIgnoreCase):
                return serviceCollection.AddSingleton<IIpAddressResolver, LocalIpResolver>();
            default:
                var ipString = ExtractIpFromParameter(parameter);
                if (IPAddress.TryParse(ipString, out var ip))
                {
                    return serviceCollection.AddSingleton<IIpAddressResolver>(new CustomIpResolver(ip));
                }

                throw new ArgumentException($"The ip address '{ipString}' is in the wrong format.");
        }
    }

    private static string ExtractIpFromParameter(string parameter) => parameter.Substring(parameter.IndexOf(':') + 1);

    private static string? GetParameter(string[] args) => args.FirstOrDefault(a => a.StartsWith(ResolveParameterPrefix, StringComparison.InvariantCultureIgnoreCase));
}