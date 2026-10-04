// <copyright file="CrywolfMonsterRole.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The role of a monster of the army of Balgass.
/// </summary>
public enum CrywolfMonsterRole
{
    /// <summary>
    /// The Dark Elf which leads a group: it marches to the goals of the group and revives its members.
    /// </summary>
    Leader,

    /// <summary>
    /// A soldier which follows the leader of its group.
    /// </summary>
    Soldier,

    /// <summary>
    /// A ballista which bombards a point of the fortress.
    /// </summary>
    Ballista,

    /// <summary>
    /// Balgass, which marches to the statue.
    /// </summary>
    Balgass,
}
