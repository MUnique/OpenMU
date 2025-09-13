// <copyright file="GuardDialogNpcPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Composition;
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
        var message = config.FindMessageFor(npc.Definition.Number);
        if (string.IsNullOrWhiteSpace(message))
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
        // Valores por defecto por número, por si no se configura nada en la UI.
        // Season 6 y clásicos:
        config.MessagesByNpcNumber[220] = "¡Alto! Mantén el orden en la ciudad.";            // Guard
        config.MessagesByNpcNumber[247] = "¡La ciudad está segura bajo nuestra vigilancia!"; // Crossbow Guard
        config.MessagesByNpcNumber[249] = "¡Mantén la paz o conocerás mi berdysh!";          // Berdysh Guard
        config.MessagesByNpcNumber[285] = "Nadie pasará sin permiso del castillo.";         // Guardian
        config.MessagesByNpcNumber[286] = "¡Listo para la batalla!";                         // Battle Guard1
        config.MessagesByNpcNumber[287] = "¡En guardia!";                                    // Battle Guard2
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
    [Display(Name = "Default Message")]
    public string DefaultMessage { get; set; } = string.Empty;

    /// <summary>
    /// Backward-compatible map of guard NPC numbers to their message.
    /// Kept hidden in UI because dictionaries are not nicely editable there.
    /// </summary>
    [Browsable(false)]
    public Dictionary<short, string> MessagesByNpcNumber { get; set; } = new();

    /// <summary>
    /// Editable list of messages per guard NPC, selectable by NPC.
    /// </summary>
    [Display(Name = "Messages")]
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    public ICollection<GuardMessageEntry> Messages { get; set; } = new List<GuardMessageEntry>();

    /// <summary>
    /// Resolves the configured message either from the list or the legacy dictionary.
    /// </summary>
    public string? FindMessageFor(short npcNumber)
    {
        var fromList = this.Messages.FirstOrDefault(m => m.Npc?.Number == npcNumber)?.Message;
        if (!string.IsNullOrWhiteSpace(fromList))
        {
            return fromList;
        }

        if (this.MessagesByNpcNumber.TryGetValue(npcNumber, out var fromDict))
        {
            return fromDict;
        }

        return null;
    }
}

/// <summary>
/// One configurable guard message entry.
/// </summary>
public class GuardMessageEntry
{
    /// <summary>
    /// Gets or sets the NPC (guard) for which the message applies.
    /// </summary>
    [Display(Name = "Guard NPC")]
    public MonsterDefinition? Npc { get; set; }

    /// <summary>
    /// Gets or sets the message which should appear as floating speech.
    /// </summary>
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}
