// <copyright file="UpdateStatsPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Collections.Frozen;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Stat update plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(UpdateStatsPlugIn097), "Stat update plugin for 0.97 clients.")]
[Guid("9914B4A3-AD76-4C3A-9C6F-48FBC9F4525C")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class UpdateStatsPlugIn097 : UpdateStatsBasePlugIn
{
    private static readonly FrozenDictionary<AttributeDefinition, Func<RemotePlayer, ValueTask>> AttributeChangeActions =
        new Dictionary<AttributeDefinition, Func<RemotePlayer, ValueTask>>
        {
            { Stats.CurrentHealth, OnCurrentHealthChangedAsync },
            { Stats.MaximumHealth, OnMaximumHealthChangedAsync },

            { Stats.CurrentMana, OnCurrentManaOrAbilityChangedAsync },
            { Stats.CurrentAbility, OnCurrentManaOrAbilityChangedAsync },
            { Stats.MaximumMana, OnMaximumManaOrAbilityChangedAsync },
            { Stats.MaximumAbility, OnMaximumManaOrAbilityChangedAsync },
        }.ToFrozenDictionary();

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatsPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateStatsPlugIn097(RemotePlayer player)
        : base(player, AttributeChangeActions)
    {
    }

    private static ValueTask OnMaximumHealthChangedAsync(RemotePlayer player)
    {
        return player.Connection.SendMaximumHealthAndShieldAsync(
            GetUShort(player.Attributes![Stats.MaximumHealth]),
            0);
    }

    private static ValueTask OnCurrentHealthChangedAsync(RemotePlayer player)
    {
        return player.Connection.SendCurrentHealthAndShieldAsync(
            GetUShort(player.Attributes![Stats.CurrentHealth]),
            0);
    }

    private static ValueTask OnMaximumManaOrAbilityChangedAsync(RemotePlayer player)
    {
        return player.Connection.SendMaximumManaAndAbilityAsync(
            GetUShort(player.Attributes![Stats.MaximumMana]),
            GetUShort(player.Attributes[Stats.MaximumAbility]));
    }

    private static ValueTask OnCurrentManaOrAbilityChangedAsync(RemotePlayer player)
    {
        return player.Connection.SendCurrentManaAndAbilityAsync(
            GetUShort(player.Attributes![Stats.CurrentMana]),
            GetUShort(player.Attributes[Stats.CurrentAbility]));
    }

    private static ushort GetUShort(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= ushort.MaxValue)
        {
            return ushort.MaxValue;
        }

        return (ushort)value;
    }

}
