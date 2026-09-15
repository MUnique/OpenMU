// <copyright file="ActorHitRecorderPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Records every hit an actor deals or receives, with the attribution the client views do not carry.
/// </summary>
/// <remarks>
/// <see cref="MUnique.OpenMU.GameLogic.Views.World.IShowHitPlugIn"/> only tells the victim how much
/// damage it took - not who dealt it - so this plugin point is the only place where attacker and
/// victim are both known. It is therefore the single source of <c>hit</c> events: neither the
/// recording view container nor a command result appends one, so a hit is recorded exactly once per
/// involved actor (once on each side when two actors fight each other).
/// </remarks>
[PlugIn]
[Display(Name = "Test actor hit recorder", Description = "Records hits dealt and received by scripted test actors in their event stream.")]
[Guid("2A7C4B18-6E5D-4C93-9F21-8D0B6A3E57C4")]
public class ActorHitRecorderPlugIn : IAttackableGotHitPlugIn
{
    /// <inheritdoc />
    public void AttackableGotHit(IAttackable attackable, IAttacker attacker, HitInfo hitInfo)
    {
        if (attacker is ScriptedPlayer dealingActor && !ReferenceEquals(attacker, attackable))
        {
            dealingActor.EventLog.Append(
                "hit",
                new ActorEventField("direction", "dealt"),
                new ActorEventField("target_id", ActorObjects.GetId(attackable)),
                new ActorEventField("target", ActorObjects.GetName(attackable)),
                new ActorEventField("target_kind", ActorObjects.GetKind(attackable)),
                new ActorEventField("health_damage", hitInfo.HealthDamage),
                new ActorEventField("shield_damage", hitInfo.ShieldDamage),
                new ActorEventField("attributes", hitInfo.Attributes.ToString()));
        }

        if (attackable is ScriptedPlayer receivingActor && !ReferenceEquals(attacker, attackable))
        {
            receivingActor.EventLog.Append(
                "hit",
                new ActorEventField("direction", "received"),
                new ActorEventField("attacker_id", ActorObjects.GetId(attacker)),
                new ActorEventField("attacker", ActorObjects.GetName(attacker)),
                new ActorEventField("attacker_kind", ActorObjects.GetKind(attacker)),
                new ActorEventField("health_damage", hitInfo.HealthDamage),
                new ActorEventField("shield_damage", hitInfo.ShieldDamage),
                new ActorEventField("attributes", hitInfo.Attributes.ToString()));
        }
    }
}
