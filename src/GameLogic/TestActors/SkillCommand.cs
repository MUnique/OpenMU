// <copyright file="SkillCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Casts a learned skill on a target.
/// </summary>
/// <param name="SkillNumber">The number of the skill.</param>
/// <param name="Target">The target's id or character name.</param>
public sealed record SkillCommand(ushort SkillNumber, string Target) : ActorCommand("skill");
