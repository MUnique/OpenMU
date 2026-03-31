// <copyright file="RequiemSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the requiem skill (book of neil) of the summoner class. Based on a chance, it may stun the target.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RequiemSkillPlugIn_Name), Description = nameof(PlugInResources.RequiemSkillPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3F1A4C8E-9B72-4D5F-A1B3-C6E7D8F90123")]
public class RequiemSkillPlugIn : IAreaSkillPlugIn
{
    private const int StunnedMagicEffectNumber = 61; // 0x3D

    private MagicEffectDefinition? _stunEffectDefinition;

    /// <inheritdoc />
    public short Key => 224;

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        this._stunEffectDefinition ??= ((Player)attacker).GameContext.Configuration.MagicEffects.First(m => m.Number == StunnedMagicEffectNumber);

        if (!target.IsAlive || !Rand.NextRandomBool(Convert.ToDouble(attacker.Attributes[Stats.StunChance])))
        {
            return;
        }

        var powerUp = attacker.Attributes.CreateElement(this._stunEffectDefinition.PowerUpDefinitions.First());
        var magicEffect = new MagicEffect(powerUp, this._stunEffectDefinition, TimeSpan.FromSeconds(3));
        await target.MagicEffectList.AddEffectAsync(magicEffect).ConfigureAwait(false);

        if (target is ISupportWalk walkSupporter && walkSupporter.IsWalking)
        {
            await walkSupporter.StopWalkingAsync().ConfigureAwait(false);

            // Since the actual coordinates could be out of sync with the client
            // coordinates, we simply update the position on the client side.
            if (walkSupporter is IObservable observable)
            {
                await observable.ForEachWorldObserverAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(walkSupporter, MoveType.Instant), true).ConfigureAwait(false);
            }
        }
    }
}