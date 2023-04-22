// <copyright file="IpAddressResolverFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

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
    /// <param name="configuration">The configuration, if available.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>The determined resolver.</returns>
    public static IIpAddressResolver CreateIpResolver(string[] args, (IpResolverType, string?)? configuration, ILoggerFactory loggerFactory)
    {
        var (resolverType, resolverParameter) = DetermineBestFittingResolver(args, configuration);

        return new ConfigurableIpResolver(resolverType, resolverParameter, loggerFactory);
    }

    /// <summary>
    /// Determines the best fitting ip resolver.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The resolver type with its parameter.</returns>
    public static (IpResolverType Type, string? Parameter) DetermineBestFittingResolver(string[] args, (IpResolverType Type, string? Parameter)? configuration = default)
    {
        return DetermineIpResolverByParameters(args)
            ?? DetermineResolverByEnvironmentVariable()
            ?? configuration
            ?? DetermineResolverByEnvironment();
    }

    private static (IpResolverType Type, string? Parameter)? DetermineIpResolverByParameters(string[] args)
    {
        var parameter = GetParameter(args)?.Trim();
        if (string.IsNullOrWhiteSpace(parameter))
        {
            return default;
        }

        return parameter! switch
        {
            { } when parameter.Equals(LoopbackIpResolve, StringComparison.InvariantCultureIgnoreCase) => (IpResolverType.Loopback, null),
            { } when parameter.Equals(PublicIpResolve, StringComparison.InvariantCultureIgnoreCase) => (IpResolverType.Public, null),
            { } when parameter.Equals(LocalIpResolve, StringComparison.InvariantCultureIgnoreCase) => ((IpResolverType, string?)?)(IpResolverType.Local, null),
            _ => (IpResolverType.Custom, ExtractIpFromParameter(parameter)),
        };
    }

    private static (IpResolverType Type, string? Parameter)? DetermineResolverByEnvironmentVariable(string variableName = "RESOLVE_IP")
    {
        if (Environment.GetEnvironmentVariable(variableName) is { } resolveIp
            && !string.IsNullOrWhiteSpace(resolveIp))
        {
            return resolveIp switch
            {
                "local" => (IpResolverType.Local, null),
                "public" => (IpResolverType.Public, null),
                "loopback" => (IpResolverType.Loopback, null),
                _ => (IpResolverType.Custom, resolveIp),
            };
        }

        return default;
    }

    private static (IpResolverType Type, string? Parameter) DetermineResolverByEnvironment()
    {
        var isDevelopmentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isDevelopmentEnvironment)
        {
            // During development, we use a loopback IP (127.127.127.127).
            return (IpResolverType.Loopback, null);
        }

        // In production our systems should expose itself with their corresponding public IPs.
        return (IpResolverType.Public, null);
    }

    private static string ExtractIpFromParameter(string parameter) => parameter.Substring(parameter.IndexOf(':') + 1);

    private static string? GetParameter(string[] args) => args.FirstOrDefault(a => a.StartsWith(ResolveParameterPrefix, StringComparison.InvariantCultureIgnoreCase));
}