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
    public const string SafeZone = "safezone";

    /// <summary>The target is farther away than the character's attack or skill range.</summary>
    public const string OutOfRange = "out_of_range";

    /// <summary>No object with that id or name is within the actor's view.</summary>
    public const string NotInView = "not_in_view";

    /// <summary>The character has not learned the requested skill.</summary>
    public const string UnknownSkill = "unknown_skill";

    /// <summary>The character cannot pay the skill's mana or ability cost.</summary>
    public const string InsufficientResources = "insufficient_resources";

    /// <summary>The path finder found no way to the requested position.</summary>
    public const string NoPath = "no_path";

    /// <summary>The target cannot be attacked (dead, not attackable, or the actor itself).</summary>
    public const string InvalidTarget = "invalid_target";

    /// <summary>The actor is dead and cannot act.</summary>
    public const string Dead = "dead";

    /// <summary>The actor is not (yet) in the world.</summary>
    public const string NotReady = "not_ready";

    /// <summary>A <c>halt</c> or a later command interrupted this one.</summary>
    public const string Interrupted = "interrupted";

    /// <summary>The requested warp list entry does not exist.</summary>
    public const string UnknownGate = "unknown_gate";

    /// <summary>The warp was refused by the game's level, zen or map rules.</summary>
    public const string WarpRefused = "warp_refused";

    /// <summary>The drop is not there any more, or could not be picked up.</summary>
    public const string PickupFailed = "pickup_failed";

    /// <summary>The command threw; the message carries the exception.</summary>
    public const string Failed = "failed";

    /// <summary>The account is already animated by an actor, a bot or a connected client.</summary>
    public const string InUse = "in_use";

    /// <summary>No actor animates the given account.</summary>
    public const string UnknownActor = "unknown_actor";

    /// <summary>This process does not host the requested game server.</summary>
    public const string UnknownServer = "unknown_server";

    /// <summary>The account or its character could not be loaded, so the actor never entered the world.</summary>
    public const string SpawnFailed = "spawn_failed";

    /// <summary>The request was not a JSON object, or its <c>cmd</c> is unknown.</summary>
    public const string BadRequest = "bad_request";

    /// <summary>
    /// The game refused the skill without telling why - its plugin returns silently when the
    /// character is stunned, in a safe zone, lacks mana or a requirement, or the target is not a
    /// legal one for that skill.
    /// </summary>
    public const string SkillRefused = "skill_refused";
}
