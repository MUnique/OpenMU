// <copyright file="UnlockMagicGladiatorAtLevel220.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unlocks the Magic Gladiator character class as soon as the first character of an account reaches level 220.
/// </summary>
[PlugIn]
[Display(Name = nameof(UnlockMagicGladiatorAtLevel220), Description = "Unlocks the Magic Gladiator character class as soon as the first character of an account reaches level 220.")]
[Guid("8C765FF3-B574-41C6-9151-ABC10D3FD959")]
public class UnlockMagicGladiatorAtLevel220 : UnlockCharacterAtLevelBase
{
    private const byte MagicGladiatorNumber = 12;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockMagicGladiatorAtLevel220"/> class.
    /// </summary>
    public UnlockMagicGladiatorAtLevel220()
        : base(MagicGladiatorNumber, 220, "Magic Gladiator")
    {
    }
}