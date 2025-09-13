// <copyright file="GuardDialogNpcPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Enables simple talk messages for guard NPCs (e.g. Crossbow Guard 247, Berdysh Guard 249).
/// Messages are configurable through the plugin configuration (Admin Panel ➜ Plugins).
/// </summary>
[PlugIn("Guard NPC Dialog", "Shows a configurable message when talking to guard NPCs.")]
[Guid("DBAC5D6D-7B0E-4C5C-8A6C-2E2E86C0E7C4")]
public class GuardDialogNpcPlugIn : IPlayerTalkToNpcPlugIn,
    ISupportCustomConfiguration<GuardDialogNpcPlugInConfiguration>,
    ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public GuardDialogNpcPlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        // Only handle guards; leave other NPCs to their own handlers.
        if (npc.Definition.ObjectKind != NpcObjectKind.Guard)
        {
            return;
        }

        var config = this.Configuration ??= CreateDefaultConfiguration();
        if (!config.MessagesByNpcNumber.TryGetValue(npc.Definition.Number, out var message) || string.IsNullOrWhiteSpace(message))
        {
            message = config.DefaultMessage;
        }

        eventArgs.HasBeenHandled = true; // Prevent default "not implemented" message.
        await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => CreateDefaultConfiguration();

    private static GuardDialogNpcPlugInConfiguration CreateDefaultConfiguration()
    {
        var config = new GuardDialogNpcPlugInConfiguration
        {
            DefaultMessage = "¡Estoy de guardia! Mantén la ciudad a salvo.",
        };

        // Common Season 6 guard NPC numbers
        config.MessagesByNpcNumber[247] = "¡La ciudad está segura bajo nuestra vigilancia!"; // Crossbow Guard
        config.MessagesByNpcNumber[249] = "¡Mantén la paz o conocerás mi berdysh!";          // Berdysh Guard

        return config;
    }
}

/// <summary>
/// Configuration for <see cref="GuardDialogNpcPlugIn"/>.
/// </summary>
public class GuardDialogNpcPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the fallback message when a guard has no specific message configured.
    /// </summary>
    public string DefaultMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the map of guard NPC numbers to their message.
    /// Example keys: 247 (Crossbow Guard), 249 (Berdysh Guard).
    /// </summary>
    public Dictionary<short, string> MessagesByNpcNumber { get; set; } = new();
}
