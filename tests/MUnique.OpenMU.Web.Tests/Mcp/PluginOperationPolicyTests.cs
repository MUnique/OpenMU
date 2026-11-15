// <copyright file="PluginOperationPolicyTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Web.AdminPanel.Mcp;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Verifies owner permissions survive the real host configuration binding path.</summary>
[TestFixture]
public sealed class PluginOperationPolicyTests
{
    /// <summary>GUID keys from JSON/environment configuration bind to exact plugin write names.</summary>
    [Test]
    public void BindsOwnerPluginPermissions()
    {
        var id = Guid.NewGuid().ToString("D");
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AdminPanel:Mcp:Enabled"] = "false",
            [$"AdminPanel:Mcp:AllowedPluginWrites:{id}:0"] = "reset_event",
        }).Build();
        var services = new ServiceCollection();
        services.AddAdminMcp(configuration);
        using var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<AdminWriteSettings>();
        Assert.That(settings.AllowedPluginWrites[id], Is.EqualTo(new[] { "reset_event" }));
        Assert.That(settings.AllowedPluginWrites.ContainsKey(Guid.NewGuid().ToString("D")), Is.False);
    }

    /// <summary>The panel plugin configuration size limit defaults to 1 MiB and can be changed by the owner.</summary>
    /// <param name="configuredLimit">The configured value, or <c>null</c> to use the default.</param>
    /// <param name="expectedLimit">The expected bound limit.</param>
    [TestCase(null, 1024 * 1024)]
    [TestCase("4194304", 4194304)]
    public void BindsPanelPluginConfigurationLimit(string? configuredLimit, int expectedLimit)
    {
        var values = new Dictionary<string, string?> { ["AdminPanel:Mcp:Enabled"] = "false" };
        if (configuredLimit is not null)
        {
            values["AdminPanel:Mcp:MaxPanelPluginConfigurationLength"] = configuredLimit;
        }

        var services = new ServiceCollection();
        services.AddAdminMcp(new ConfigurationBuilder().AddInMemoryCollection(values).Build());
        using var provider = services.BuildServiceProvider();
        Assert.That(provider.GetRequiredService<AdminWriteSettings>().MaxPanelPluginConfigurationLength, Is.EqualTo(expectedLimit));
    }

    /// <summary>A non-positive panel plugin configuration size limit is rejected at startup, even when MCP is off.</summary>
    [Test]
    public void RejectsNonPositivePanelPluginConfigurationLimit()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AdminPanel:Mcp:Enabled"] = "false",
            ["AdminPanel:Mcp:MaxPanelPluginConfigurationLength"] = "0",
        }).Build();
        Assert.Throws<ArgumentException>(() => new ServiceCollection().AddAdminMcp(configuration));
    }
}
