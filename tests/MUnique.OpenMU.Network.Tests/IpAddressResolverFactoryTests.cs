// <copyright file="IpAddressResolverFactoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests;

using System.Net;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Tests for <see cref="IpAddressResolverFactory"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class IpAddressResolverFactoryTests
{
    private readonly IPAddress _expectedLoopbackAddress = IPAddress.Parse("127.127.127.127");

    private string? _originalResolveIpEnvironmentVariable;
    private string? _originalAspNetCoreEnvironmentVariable;

    /// <summary>
    /// Captures and clears the environment variables which influence resolver determination.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._originalResolveIpEnvironmentVariable = Environment.GetEnvironmentVariable("RESOLVE_IP");
        this._originalAspNetCoreEnvironmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Environment.SetEnvironmentVariable("RESOLVE_IP", null);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
    }

    /// <summary>
    /// Restores the environment variables which were changed during a test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("RESOLVE_IP", this._originalResolveIpEnvironmentVariable);
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", this._originalAspNetCoreEnvironmentVariable);
    }

    /// <summary>
    /// Tests if runtime reconfiguration is ignored when the resolver was configured by startup parameters.
    /// </summary>
    /// <returns>The asynchronous operation.</returns>
    [Test]
    public async Task ResolverConfiguredByStartupParameterCannotBeOverriddenAsync()
    {
        var resolver = (ConfigurableIpResolver)IpAddressResolverFactory.CreateIpResolver(new[] { "-resolveIP:loopback" }, (IpResolverType.Public, null), new NullLoggerFactory());

        resolver.Configure(IpResolverType.Custom, "1.2.3.4");
        var resolvedAddress = await resolver.ResolveIPv4Async().ConfigureAwait(false);

        Assert.That(resolvedAddress, Is.EqualTo(this._expectedLoopbackAddress));
    }

    /// <summary>
    /// Tests if runtime reconfiguration is ignored when the resolver was configured by environment variable.
    /// </summary>
    /// <returns>The asynchronous operation.</returns>
    [Test]
    public async Task ResolverConfiguredByEnvironmentVariableCannotBeOverriddenAsync()
    {
        Environment.SetEnvironmentVariable("RESOLVE_IP", "loopback");
        var resolver = (ConfigurableIpResolver)IpAddressResolverFactory.CreateIpResolver(Array.Empty<string>(), (IpResolverType.Public, null), new NullLoggerFactory());

        resolver.Configure(IpResolverType.Custom, "1.2.3.4");
        var resolvedAddress = await resolver.ResolveIPv4Async().ConfigureAwait(false);

        Assert.That(resolvedAddress, Is.EqualTo(this._expectedLoopbackAddress));
    }

    /// <summary>
    /// Tests if runtime reconfiguration still works when the resolver was configured by persisted settings.
    /// </summary>
    /// <returns>The asynchronous operation.</returns>
    [Test]
    public async Task ResolverConfiguredByPersistedSettingsCanBeOverriddenAsync()
    {
        var resolver = (ConfigurableIpResolver)IpAddressResolverFactory.CreateIpResolver(Array.Empty<string>(), (IpResolverType.Loopback, null), new NullLoggerFactory());

        resolver.Configure(IpResolverType.Custom, "1.2.3.4");
        var resolvedAddress = await resolver.ResolveIPv4Async().ConfigureAwait(false);

        Assert.That(resolvedAddress, Is.EqualTo(IPAddress.Parse("1.2.3.4")));
    }
}
