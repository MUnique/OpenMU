// <copyright file="DeActivateMagicEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IActivateMagicEffectPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DeActivateMagicEffectPlugIn_Name), Description = nameof(PlugInResources.DeActivateMagicEffectPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("67642604-8abb-44b9-a668-989cb3b28e89")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class DeActivateMagicEffectPlugIn : IActivateMagicEffectPlugIn, IDeactivateMagicEffectPlugIn
{
    private static readonly ReadOnlyDictionary<AttributeDefinition, EffectItemConsumption.EffectType> EffectTypeMapping = new(
        new Dictionary<AttributeDefinition, EffectItemConsumption.EffectType>
        {
            { Stats.AttackSpeedAny, EffectItemConsumption.EffectType.AttackSpeed },
            { Stats.BaseDamageBonus, EffectItemConsumption.EffectType.Damage },
            { Stats.DefenseBase, EffectItemConsumption.EffectType.Defense },
            { Stats.MaximumHealth, EffectItemConsumption.EffectType.MaximumHealth },
            { Stats.MaximumMana, EffectItemConsumption.EffectType.MaximumMana },
        });

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeActivateMagicEffectPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DeActivateMagicEffectPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask ActivateMagicEffectAsync(MagicEffect effect, IAttackable affectedObject)
    {
        return this.SendMagicEffectStatusAsync(effect, affectedObject, true, effect.Definition.SendDuration ? effect.Duration : TimeSpan.Zero);
    }

    /// <inheritdoc/>
    public ValueTask DeactivateMagicEffectAsync(MagicEffect effect, IAttackable affectedObject)
    {
        return this.SendMagicEffectStatusAsync(effect, affectedObject, false, TimeSpan.Zero);
    }

    /// <summary>
    /// Gets the connection to send an effect to, if the player is connected and the effect has a number which the client knows.
    /// Each entry point checks this once before it sends.
    /// </summary>
    /// <param name="player">The player who gets the packet.</param>
    /// <param name="effect">The effect.</param>
    /// <param name="connection">The connection of the player.</param>
    /// <returns><c>true</c>, if the effect can be sent.</returns>
    internal static bool TryGetConnection(RemotePlayer player, MagicEffect effect, [NotNullWhen(true)] out IConnection? connection)
    {
        connection = player.Connection is { Connected: true } c && effect.Definition.Number > 0 ? c : null;
        return connection is not null;
    }

    /// <summary>
    /// Sends the short <see cref="MagicEffectStatus"/> of an effect to the player.
    /// </summary>
    /// <param name="connection">The connection of the player, from <see cref="TryGetConnection"/>.</param>
    /// <param name="player">The player who gets the packet.</param>
    /// <param name="effect">The effect.</param>
    /// <param name="affectedObject">The object which has the effect.</param>
    /// <param name="isActive">Whether the effect is active.</param>
    /// <returns>The task.</returns>
    internal static ValueTask SendShortMagicEffectStatusAsync(IConnection connection, RemotePlayer player, MagicEffect effect, IAttackable affectedObject, bool isActive)
    {
        return connection.SendMagicEffectStatusAsync(isActive, affectedObject.GetId(player), (byte)effect.Id);
    }

    /// <summary>
    /// Sends the effect of a consumed item, for each of its power-ups of a kind which the client knows (<see cref="EffectTypeMapping"/>).
    /// </summary>
    /// <param name="connection">The connection of the player who has the effect.</param>
    /// <param name="effect">The effect.</param>
    /// <param name="duration">The duration which is sent.</param>
    /// <returns><c>true</c>, if a packet was sent.</returns>
    internal static async ValueTask<bool> SendEffectItemConsumptionAsync(IConnection connection, MagicEffect effect, TimeSpan duration)
    {
        bool effectWasSent = false;
        foreach (var powerUpDefinition in effect.Definition.PowerUpDefinitions)
        {
            if (powerUpDefinition.TargetAttribute is { } targetAttribute
                && EffectTypeMapping.TryGetValue(targetAttribute, out var effectType))
            {
                var origin = EffectItemConsumption.EffectOrigin.HalloweenAndCherryBlossomEvent; // Basically, all normal consumable items which add effects
                var action = EffectItemConsumption.EffectAction.Add;
                await connection.SendEffectItemConsumptionAsync(origin, effectType, action, (uint)duration.TotalSeconds, (byte)effect.Definition.Number).ConfigureAwait(false);
                effectWasSent = true;
            }
        }

        return effectWasSent;
    }

    private async ValueTask SendMagicEffectStatusAsync(MagicEffect effect, IAttackable affectedObject, bool isActive, TimeSpan duration)
    {
        if (!TryGetConnection(this._player, effect, out var connection))
        {
            return;
        }

        if (isActive
            && affectedObject == this._player
            && await SendEffectItemConsumptionAsync(connection, effect, duration).ConfigureAwait(false))
        {
            return;
        }

        await SendShortMagicEffectStatusAsync(connection, this._player, effect, affectedObject, isActive).ConfigureAwait(false);
    }
}