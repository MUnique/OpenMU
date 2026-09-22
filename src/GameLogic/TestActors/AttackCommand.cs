// <copyright file="AttackCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Performs plain attacks against an object in view.
/// </summary>
/// <param name="Target">The target's id or character name.</param>
/// <param name="Times">How many attacks to perform.</param>
/// <param name="IntervalMs">The delay between two attacks, in milliseconds.</param>
public sealed record AttackCommand(string Target, int Times, int IntervalMs) : ActorCommand("attack");
