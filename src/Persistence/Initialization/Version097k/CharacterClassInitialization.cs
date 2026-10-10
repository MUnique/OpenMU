// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097k;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initialization of character classes data for version 0.97k.
/// </summary>
/// <remarks>
/// Compared to version 0.95d, the dark wizard, dark knight and fairy elf can evolve to their second class.
/// </remarks>
internal class CharacterClassInitialization : Initialization.CharacterClasses.CharacterClassInitialization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterClassInitialization"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CharacterClassInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override bool UseClassicPvp => true;

    /// <inheritdoc />
    public override void Initialize()
    {
        var bladeKnight = this.CreateDarkKnight(CharacterClassNumber.BladeKnight, LocalizedString.FromResource(() => CharacterClassNames.BladeKnight), false, null, false);
        this.CreateDarkKnight(CharacterClassNumber.DarkKnight, LocalizedString.FromResource(() => CharacterClassNames.DarkKnight), false, bladeKnight, true);

        var soulMaster = this.CreateDarkWizard(CharacterClassNumber.SoulMaster, LocalizedString.FromResource(() => CharacterClassNames.SoulMaster), false, null, false);
        this.CreateDarkWizard(CharacterClassNumber.DarkWizard, LocalizedString.FromResource(() => CharacterClassNames.DarkWizard), false, soulMaster, true);

        var museElf = this.CreateFairyElf(CharacterClassNumber.MuseElf, LocalizedString.FromResource(() => CharacterClassNames.MuseElf), false, null, false);
        this.CreateFairyElf(CharacterClassNumber.FairyElf, LocalizedString.FromResource(() => CharacterClassNames.FairyElf), false, museElf, true);

        this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, LocalizedString.FromResource(() => CharacterClassNames.MagicGladiator), false, null, true);
    }
}
