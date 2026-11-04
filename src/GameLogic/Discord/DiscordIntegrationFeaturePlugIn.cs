// <copyright file="DiscordIntegrationFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Discord;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Discord integration of the game client: it tells clients which ask for it how the server is connected
/// to Discord, lets the players link their account from the client, and sends them the messages which were
/// written in Discord as such. When it's deactivated, the clients get no Discord integration info.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DiscordIntegrationFeaturePlugIn_Name), Description = nameof(PlugInResources.DiscordIntegrationFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("26096524-5D7E-4D13-925C-04AAC0340B70")]
public class DiscordIntegrationFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<DiscordIntegrationConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public DiscordIntegrationConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the configuration of the Discord integration of the game context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration; <c>null</c>, if the Discord integration is deactivated.</returns>
    public static DiscordIntegrationConfiguration? GetConfiguration(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<DiscordIntegrationFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? new DiscordIntegrationConfiguration()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new DiscordIntegrationConfiguration();
    }
}
