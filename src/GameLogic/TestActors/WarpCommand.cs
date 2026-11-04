// <copyright file="WarpCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Uses an entry of the game's warp list.
/// </summary>
/// <param name="GateNumber">The index of the warp list entry.</param>
public sealed record WarpCommand(int GateNumber) : ActorCommand("warp");
