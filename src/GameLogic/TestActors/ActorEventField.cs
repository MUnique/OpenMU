// <copyright file="ActorEventField.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// One field of an <see cref="ActorEvent"/>. Values are deliberately limited to strings, numbers and
/// booleans - an event never holds a reference to a game entity, so a reader can keep it forever
/// without pinning a player, monster or item.
/// </summary>
/// <param name="Name">The field name as it appears in the JSON output.</param>
/// <param name="Value">The value; <c>null</c> is written as JSON <c>null</c>.</param>
public readonly record struct ActorEventField(string Name, object? Value);
