// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initialization of character classes data for Version 0.97d.
/// </summary>
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
        var bladeKnight = this.CreateDarkKnight(CharacterClassNumber.BladeKnight, "Blade Knight", false, null, false);
        this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", false, bladeKnight, true);

        var soulMaster = this.CreateDarkWizard(CharacterClassNumber.SoulMaster, "Soul Master", false, null, false);
        this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", false, soulMaster, true);

        var museElf = this.CreateFairyElf(CharacterClassNumber.MuseElf, "Muse Elf", false, null, false);
        this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", false, museElf, true);

        this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, null, true);
    }
}
