// <copyright file="ActorState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Turns an actor and its surroundings into the flat dictionaries the control endpoint answers with.
/// </summary>
public static class ActorState
{
    /// <summary>
    /// Describes an actor as one result field, as <c>spawn</c> and <c>list</c> report it.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="actor">The actor.</param>
    /// <returns>The field.</returns>
    public static ActorEventField Describe(string loginName, ScriptedPlayer actor)
        => new("actor", Summary(loginName, actor));

    /// <summary>
    /// The short form: who the actor is and where it stands.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="actor">The actor.</param>
    /// <returns>The summary.</returns>
    public static Dictionary<string, object?> Summary(string loginName, ScriptedPlayer actor)
    {
        return new Dictionary<string, object?>
        {
            ["actor"] = loginName,
            ["character"] = actor.Name,
            ["map"] = actor.CurrentMap?.Definition.Name.ToString() ?? string.Empty,
            ["map_number"] = actor.CurrentMap?.Definition.Number ?? -1,
            ["x"] = actor.Position.X,
            ["y"] = actor.Position.Y,
            ["alive"] = actor.IsAlive,
        };
    }

    /// <summary>
    /// The full state a scenario asserts on.
    /// </summary>
    /// <param name="loginName">The login name of the account.</param>
    /// <param name="actor">The actor.</param>
    /// <returns>The state.</returns>
    public static Dictionary<string, object?> Full(string loginName, ScriptedPlayer actor)
    {
        var attributes = actor.Attributes;
        var state = Summary(loginName, actor);
        state["class"] = actor.SelectedCharacter?.CharacterClass?.Name.ToString() ?? string.Empty;
        state["level"] = (int)(attributes?[Stats.Level] ?? 0);
        state["master_level"] = (int)(attributes?[Stats.MasterLevel] ?? 0);
        state["health"] = (int)(attributes?[Stats.CurrentHealth] ?? 0);
        state["max_health"] = (int)(attributes?[Stats.MaximumHealth] ?? 0);
        state["shield"] = (int)(attributes?[Stats.CurrentShield] ?? 0);
        state["max_shield"] = (int)(attributes?[Stats.MaximumShield] ?? 0);
        state["mana"] = (int)(attributes?[Stats.CurrentMana] ?? 0);
        state["max_mana"] = (int)(attributes?[Stats.MaximumMana] ?? 0);
        state["ability"] = (int)(attributes?[Stats.CurrentAbility] ?? 0);
        state["max_ability"] = (int)(attributes?[Stats.MaximumAbility] ?? 0);
        state["zen"] = actor.Money;
        state["hero_state"] = actor.SelectedCharacter?.State.ToString() ?? string.Empty;
        state["target"] = actor.LastAttackedTarget.TryGetTarget(out var target) && target is not null
            ? ActorObjects.GetName(target)
            : null;
        state["party"] = actor.Party?.PartyList.Select(m => m.Name).ToList() ?? [];
        state["in_view"] = Nearby(actor);
        state["last_seq"] = actor.EventLog.LastSequence;
        return state;
    }

    /// <summary>
    /// The objects currently in the actor's view.
    /// </summary>
    /// <param name="actor">The actor.</param>
    /// <returns>The objects, with id, kind, name, position and - where it applies - the alive flag.</returns>
    public static List<Dictionary<string, object?>> Nearby(ScriptedPlayer actor)
    {
        var result = new List<Dictionary<string, object?>>();
        if (actor.CurrentMap is not { } map)
        {
            return result;
        }

        var position = actor.Position;
        var range = actor.InfoRange;

        foreach (var attackable in map.GetAttackablesInRange(position, range))
        {
            if (ReferenceEquals(attackable, actor))
            {
                continue;
            }

            result.Add(Describe(attackable, attackable.IsAlive));
        }

        foreach (var npc in map.GetNpcsInRange(position, range))
        {
            if (npc is IAttackable)
            {
                // Already listed above; monsters are attackable NPCs.
                continue;
            }

            result.Add(Describe(npc, null));
        }

        foreach (var drop in map.GetDropsInRange(position, range))
        {
            result.Add(Describe(drop, null));
        }

        return result;
    }

    private static Dictionary<string, object?> Describe(ILocateable gameObject, bool? alive)
    {
        return new Dictionary<string, object?>
        {
            ["id"] = gameObject.Id,
            ["kind"] = ActorObjects.GetKind(gameObject),
            ["name"] = ActorObjects.GetName(gameObject),
            ["x"] = gameObject.Position.X,
            ["y"] = gameObject.Position.Y,
            ["alive"] = alive,
        };
    }
}
