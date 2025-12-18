// <copyright file="UnlockDarkLordAtLevel250.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Unlocks the Dark Lord character class as soon as the first character of an account reaches level 250.
/// </summary>
[PlugIn]
[Display(Name = nameof(UnlockDarkLordAtLevel250), Description = "Unlocks the Dark Lord character class as soon as the first character of an account reaches level 250.")]
[Guid("2DFFD75A-765D-4FA7-93DD-9890CA0F04F0")]
public class UnlockDarkLordAtLevel250 : UnlockCharacterAtLevelBase
{
    private const byte DarkLordNumber = 16;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockDarkLordAtLevel250"/> class.
    /// </summary>
    public UnlockDarkLordAtLevel250()
        : base(DarkLordNumber, 250, "Dark Lord")
    {
    }
}