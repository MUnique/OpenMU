// <copyright file="PickupCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Picks up a dropped item which is in view.
/// </summary>
/// <param name="DropId">The id of the drop.</param>
public sealed record PickupCommand(ushort DropId) : ActorCommand("pickup");
