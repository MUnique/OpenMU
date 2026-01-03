// <copyright file="CharacterClassInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
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
        var bladeMaster = this.CreateBladeMaster();
        var bladeKnight = this.CreateBladeKnight(bladeMaster);
        this.CreateDarkKnight(CharacterClassNumber.DarkKnight, "Dark Knight", false, bladeKnight, true);

        var grandMaster = this.CreateGrandMaster();
        var soulMaster = this.CreateSoulMaster(grandMaster);
        this.CreateDarkWizard(CharacterClassNumber.DarkWizard, "Dark Wizard", false, soulMaster, true);

        var highElf = this.CreateHighElf();
        var museElf = this.CreateMuseElf(highElf);
        this.CreateFairyElf(CharacterClassNumber.FairyElf, "Fairy Elf", false, museElf, true);

        this.CreateMagicGladiator(CharacterClassNumber.MagicGladiator, "Magic Gladiator", false, null, true);
    }

    private CharacterClass CreateBladeMaster()
    {
        var result = this.CreateDarkKnight(CharacterClassNumber.BladeMaster, "Blade Master", true, null, false);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
        return result;
    }

    private CharacterClass CreateBladeKnight(CharacterClass bladeMaster)
    {
        return this.CreateDarkKnight(CharacterClassNumber.BladeKnight, "Blade Knight", false, bladeMaster, false);
    }

    private CharacterClass CreateGrandMaster()
    {
        var result = this.CreateDarkWizard(CharacterClassNumber.GrandMaster, "Grand Master", true, null, false);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
        return result;
    }

    private CharacterClass CreateSoulMaster(CharacterClass grandMaster)
    {
        return this.CreateDarkWizard(CharacterClassNumber.SoulMaster, "Soul Master", false, grandMaster, false);
    }

    private CharacterClass CreateHighElf()
    {
        var result = this.CreateFairyElf(CharacterClassNumber.HighElf, "High Elf", true, null, false);
        result.StatAttributes.Add(this.CreateStatAttributeDefinition(Stats.MasterLevel, 0, false));
        return result;
    }

    private CharacterClass CreateMuseElf(CharacterClass highElf)
    {
        return this.CreateFairyElf(CharacterClassNumber.MuseElf, "Muse Elf", false, highElf, false);
    }
}
