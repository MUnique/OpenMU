// <copyright file="ActorObjects.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Describes the objects of the game world the way the actor's event stream and command results
/// refer to them: by id, by name and by kind, never by reference.
/// </summary>
public static class ActorObjects
{
    /// <summary>The kind of a scripted actor.</summary>
    public static readonly string ActorKind = "actor";

    /// <summary>The kind of a server-side population bot.</summary>
    public static readonly string BotKind = "bot";

    /// <summary>The kind of a human player, connected or in an offline session.</summary>
    public static readonly string PlayerKind = "player";

    /// <summary>The kind of a monster.</summary>
    public static readonly string MonsterKind = "monster";

    /// <summary>The kind of a non-attackable NPC.</summary>
    public static readonly string NpcKind = "npc";

    /// <summary>The kind of a dropped item.</summary>
    public static readonly string ItemKind = "item";

    /// <summary>The kind of anything else.</summary>
    public static readonly string UnknownKind = "unknown";

    /// <summary>
    /// Gets the kind of the given game object.
    /// </summary>
    /// <param name="gameObject">The object; may be <c>null</c>.</param>
    /// <returns>One of the kind constants of this class.</returns>
    public static string GetKind(object? gameObject)
    {
        return gameObject switch
        {
            ScriptedPlayer => ActorKind,
            Player { Account.IsBot: true } => BotKind,
            Player => PlayerKind,
            Monster => MonsterKind,
            NonPlayerCharacter => NpcKind,
            DroppedItem => ItemKind,
            DroppedMoney => ItemKind,
            _ => UnknownKind,
        };
    }

    /// <summary>
    /// Gets the name a scenario can address the given game object by.
    /// </summary>
    /// <param name="gameObject">The object; may be <c>null</c>.</param>
    /// <returns>The character name, the monster designation, the item name, or an empty string.</returns>
    public static string GetName(object? gameObject)
    {
        return gameObject switch
        {
            Player player => player.Name,
            NonPlayerCharacter npc => npc.Definition.Designation.ToString() ?? string.Empty,
            DroppedItem droppedItem => droppedItem.Item.ToString() ?? string.Empty,
            DroppedMoney droppedMoney => $"{droppedMoney.Amount} Zen",
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Gets the id of the given game object, which is unique on its map while it exists.
    /// </summary>
    /// <param name="gameObject">The object; may be <c>null</c>.</param>
    /// <returns>The id, or 0 when the object has none.</returns>
    public static ushort GetId(object? gameObject)
    {
        return gameObject is IIdentifiable identifiable ? identifiable.Id : (ushort)0;
    }
}
