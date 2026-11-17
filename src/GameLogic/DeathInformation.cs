// <copyright file="DeathInformation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Contains information about the last death of the object.
/// </summary>
/// <param name="KillerId">The id of the killer.</param>
/// <param name="KillerName">The name of the killer.</param>
/// <param name="FinalHit">The hit info of the final/lethal hit.</param>
/// <param name="SkillNumber">The number of the used skill.</param>
public record DeathInformation(ushort KillerId, string KillerName, HitInfo FinalHit, short SkillNumber);
