// <copyright file="ActorEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// One recorded observation of a <see cref="ScriptedPlayer"/>: what the game would have sent to the
/// client of that character, flattened into names, ids and numbers so it can be written as one JSON
/// line and asserted on from a shell.
/// </summary>
/// <param name="Seq">
/// The sequence number, strictly increasing per actor. Readers use it to fetch only what they have
/// not seen yet.
/// </param>
/// <param name="Utc">The time the event was recorded.</param>
/// <param name="Type">The event type, e.g. <c>hit</c>, <c>stat</c>, <c>killed</c>, <c>chat</c>.</param>
/// <param name="Fields">The type specific fields, in the order they should be written.</param>
public sealed record ActorEvent(long Seq, DateTime Utc, string Type, IReadOnlyList<ActorEventField> Fields);
