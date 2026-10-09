// <copyright file="UpdateStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Collections.Frozen;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateStatsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.UpdateStatsPlugIn_Name), Description = nameof(PlugInResources.UpdateStatsPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2A8BFB0C-2AFF-4A52-B390-5A68D5C5F26A")]
public class UpdateStatsPlugIn : UpdateStatsBasePlugIn
{
    private static readonly FrozenDictionary<AttributeDefinition, Func<RemotePlayer, ValueTask>> AttributeChangeActions = new Dictionary<AttributeDefinition, Func<RemotePlayer, ValueTask>>
    {
        { Stats.CurrentHealth, OnCurrentHealthOrShieldChangedAsync },
        { Stats.CurrentShield, OnCurrentHealthOrShieldChangedAsync },
        { Stats.MaximumHealth, OnMaximumHealthOrShieldChangedAsync },
        { Stats.MaximumShield, OnMaximumHealthOrShieldChangedAsync },
        { Stats.CurrentMana, OnCurrentManaOrAbilityChangedAsync },
        { Stats.CurrentAbility, OnCurrentManaOrAbilityChangedAsync },
        { Stats.MaximumMana, OnMaximumManaOrAbilityChangedAsync },
        { Stats.MaximumAbility, OnMaximumManaOrAbilityChangedAsync },
    }.ToFrozenDictionary();

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateStatsPlugIn(RemotePlayer player)
        : base(player, AttributeChangeActions)
    {
    }

    private static async ValueTask OnMaximumHealthOrShieldChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendMaximumHealthAndShieldAsync(
            player.Attributes![Stats.MaximumHealth].ToUInt16Clamped(),
            player.Attributes[Stats.MaximumShield].ToUInt16Clamped()).ConfigureAwait(false);
    }

    private static async ValueTask OnMaximumManaOrAbilityChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendMaximumManaAndAbilityAsync(
            player.Attributes![Stats.MaximumMana].ToUInt16Clamped(),
            player.Attributes[Stats.MaximumAbility].ToUInt16Clamped()).ConfigureAwait(false);
    }

    private static async ValueTask OnCurrentHealthOrShieldChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendCurrentHealthAndShieldAsync(
            player.Attributes![Stats.CurrentHealth].ToUInt16Clamped(),
            player.Attributes[Stats.CurrentShield].ToUInt16Clamped()).ConfigureAwait(false);
    }

    private static async ValueTask OnCurrentManaOrAbilityChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendCurrentManaAndAbilityAsync(
            player.Attributes![Stats.CurrentMana].ToUInt16Clamped(),
            player.Attributes[Stats.CurrentAbility].ToUInt16Clamped()).ConfigureAwait(false);
    }
}