// <copyright file="IDestructible.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Contains information about the last death of the destructible.
/// </summary>
/// <param name="KillerId">The id of the killer.</param>
/// <param name="KillerName">The name of the killer.</param>
/// <param name="FinalHit">The hit info of the final/lethal hit.</param>
/// <param name="SkillNumber">The number of the used skill.</param>
/// <param name="ObjectNumber">The number of the destructible.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "The names are affecting the property names of the record. We want upper case there.")]
public record DestructibleDeathInformation(ushort KillerId, string KillerName, HitInfo FinalHit, short SkillNumber, short ObjectNumber);

/// <summary>
/// Interface for an object which is destructible.
/// </summary>
public interface IDestructible : IAttackable
{
    /// <summary>
    /// Gets the information about the last death.
    /// </summary>
    DestructibleDeathInformation? DestructibleLastDeath { get; }
}