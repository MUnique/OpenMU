// <copyright file="ImperialGuardianFragmentDrop.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// A secromicon fragment, which is dropped by a boss of the imperial guardian event.
/// </summary>
public class ImperialGuardianFragmentDrop
{
    /// <summary>
    /// Gets or sets the number of the boss.
    /// </summary>
    public short MonsterNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of the fragment item in the item group 14.
    /// </summary>
    public short ItemNumber { get; set; }
}
