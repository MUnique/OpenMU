// <copyright file="DeActivateMagicEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IActivateMagicEffectPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("De & ActivateMagicEffectPlugIn", "The default implementation of the IActivateMagicEffectPlugIn and IDeactivateMagicEffectPlugIn which is forwarding everything to the game client with specific data packets.")]
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

    private async ValueTask SendMagicEffectStatusAsync(MagicEffect effect, IAttackable affectedObject, bool isActive, TimeSpan duration)
    {
        if (!(this._player.Connection?.Connected ?? false)
            || effect.Definition.Number <= 0)
        {
            return;
        }

        bool effectWasSent = false;
        var objectId = affectedObject.GetId(this._player);
        if (isActive && affectedObject == this._player)
        {
            foreach (var powerUpDefinition in effect.Definition.PowerUpDefinitions)
            {
                if (powerUpDefinition.TargetAttribute is { } targetAttribute
                    && EffectTypeMapping.TryGetValue(targetAttribute, out var effectType))
                {
                    var origin = EffectItemConsumption.EffectOrigin.HalloweenAndCherryBlossomEvent; // Basically, all normal consumable items which add effects
                    var action = EffectItemConsumption.EffectAction.Add;
                    await this._player.Connection.SendEffectItemConsumptionAsync(origin, effectType, action, (uint)duration.TotalSeconds, (byte)effect.Definition.Number).ConfigureAwait(false);
                    effectWasSent = true;
                }
            }
        }

        if (!effectWasSent)
        {
            await this._player.Connection.SendMagicEffectStatusAsync(isActive, objectId, (byte)effect.Id).ConfigureAwait(false);
        }
    }
}