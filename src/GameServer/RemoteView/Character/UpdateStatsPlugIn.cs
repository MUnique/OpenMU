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
[PlugIn(nameof(UpdateStatsPlugIn), "The default implementation of the IUpdateStatsPlugIn which is forwarding everything to the game client with specific data packets.")]
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
            (ushort)Math.Max(player.Attributes![Stats.MaximumHealth], 0f),
            (ushort)Math.Max(player.Attributes[Stats.MaximumShield], 0f)).ConfigureAwait(false);
    }

    private static async ValueTask OnMaximumManaOrAbilityChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendMaximumManaAndAbilityAsync(
            (ushort)Math.Max(player.Attributes![Stats.MaximumMana], 0f),
            (ushort)Math.Max(player.Attributes[Stats.MaximumAbility], 0f)).ConfigureAwait(false);
    }

    private static async ValueTask OnCurrentHealthOrShieldChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendCurrentHealthAndShieldAsync(
            (ushort)Math.Max(player.Attributes![Stats.CurrentHealth], 0f),
            (ushort)Math.Max(player.Attributes[Stats.CurrentShield], 0f)).ConfigureAwait(false);
    }

    private static async ValueTask OnCurrentManaOrAbilityChangedAsync(RemotePlayer player)
    {
        await player.Connection.SendCurrentManaAndAbilityAsync(
            (ushort)Math.Max(player.Attributes![Stats.CurrentMana], 0f),
            (ushort)Math.Max(player.Attributes[Stats.CurrentAbility], 0f)).ConfigureAwait(false);
    }
}