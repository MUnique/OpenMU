// <copyright file="NpcTalkPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for a plugin which handles talking to the NPC of its <see cref="NpcTalkPlugInConfiguration"/>.
/// </summary>
public abstract class NpcTalkPlugInBase : IPlayerTalkToNpcPlugIn, ISupportCustomConfiguration<NpcTalkPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public NpcTalkPlugInConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the number of the NPC which is configured for a new database.
    /// </summary>
    public abstract short DefaultNpcNumber { get; }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (this.IsConfiguredNpc(npc))
        {
            await this.PlayerTalksToConfiguredNpcAsync(player, npc, eventArgs).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new NpcTalkPlugInConfiguration();

    /// <summary>
    /// Creates the default configuration, which references the NPC with the <see cref="DefaultNpcNumber"/>.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The default configuration.</returns>
    public NpcTalkPlugInConfiguration CreateDefaultConfig(GameConfiguration gameConfiguration)
    {
        return new NpcTalkPlugInConfiguration
        {
            Npc = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == this.DefaultNpcNumber),
        };
    }

    /// <summary>
    /// Determines whether the NPC is the configured NPC of this plugin.
    /// </summary>
    /// <param name="npc">The NPC.</param>
    /// <returns><c>true</c>, if the NPC is the configured NPC of this plugin.</returns>
    public bool IsConfiguredNpc(NonPlayerCharacter npc)
    {
        return this.Configuration?.Npc is { } configuredNpc
               && npc.Definition.Number == configuredNpc.Number;
    }

    /// <summary>
    /// Handles talking to the configured NPC.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="npc">The NPC, which is the configured NPC of this plugin.</param>
    /// <param name="eventArgs">The <see cref="NpcTalkEventArgs"/> instance containing the event data.</param>
    /// <returns>The task.</returns>
    protected abstract ValueTask PlayerTalksToConfiguredNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs);
}
