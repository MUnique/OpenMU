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
}
