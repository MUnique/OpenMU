// <copyright file="ActorErrorCodes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// The failure codes a command can answer with. They are part of the protocol: test scripts
/// branch on them.
/// </summary>
public static class ActorErrorCodes
{
    /// <summary>The actor or its target stands in a safe zone, where attacks are forbidden.</summary>
    public static readonly string SafeZone = "safezone";

    /// <summary>The target is farther away than the character's attack or skill range.</summary>
    public static readonly string OutOfRange = "out_of_range";

    /// <summary>No object with that id or name is within the actor's view.</summary>
    public static readonly string NotInView = "not_in_view";

    /// <summary>The character has not learned the requested skill.</summary>
    public static readonly string UnknownSkill = "unknown_skill";

    /// <summary>The character cannot pay the skill's mana or ability cost.</summary>
    public static readonly string InsufficientResources = "insufficient_resources";

    /// <summary>
    /// The path finder found no way to the requested position, or the walk was stopped by a blocked
    /// tile before reaching it (the result then carries the position actually reached).
    /// </summary>
    public static readonly string NoPath = "no_path";

    /// <summary>The target cannot be attacked (dead, not attackable, or the actor itself).</summary>
    public static readonly string InvalidTarget = "invalid_target";

    /// <summary>The actor is dead and cannot act.</summary>
    public static readonly string Dead = "dead";

    /// <summary>The actor is not (yet) in the world.</summary>
    public static readonly string NotReady = "not_ready";

    /// <summary>A <c>halt</c> or a later command interrupted this one.</summary>
    public static readonly string Interrupted = "interrupted";

    /// <summary>The requested warp list entry does not exist.</summary>
    public static readonly string UnknownGate = "unknown_gate";

    /// <summary>The warp was refused by the game's level, zen or map rules.</summary>
    public static readonly string WarpRefused = "warp_refused";

    /// <summary>The drop is not there any more, or could not be picked up.</summary>
    public static readonly string PickupFailed = "pickup_failed";

    /// <summary>The command threw; the message carries the exception.</summary>
    public static readonly string Failed = "failed";

    /// <summary>The account is already animated by an actor, a bot or a connected client.</summary>
    public static readonly string InUse = "in_use";

    /// <summary>No actor animates the given account.</summary>
    public static readonly string UnknownActor = "unknown_actor";

    /// <summary>This process does not host the requested game server.</summary>
    public static readonly string UnknownServer = "unknown_server";

    /// <summary>The account or its character could not be loaded, so the actor never entered the world.</summary>
    public static readonly string SpawnFailed = "spawn_failed";

    /// <summary>The request was not a JSON object, or its <c>cmd</c> is unknown.</summary>
    public static readonly string BadRequest = "bad_request";

    /// <summary>
    /// The game refused the skill without telling why - its plugin returns silently when the
    /// character is stunned, in a safe zone, lacks mana or a requirement, or the target is not a
    /// legal one for that skill.
    /// </summary>
    public static readonly string SkillRefused = "skill_refused";
}
