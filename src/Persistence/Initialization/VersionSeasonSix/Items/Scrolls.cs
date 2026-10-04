// <copyright file="Scrolls.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initializer for scroll items which allow a character to learn <see cref="Skill"/>s.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
public class Scrolls : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scrolls"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Scrolls(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes the scroll data.
    /// </summary>
    /// <remarks>
    /// Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
    /// Replace by: this.CreateScroll($1, TODO, $5, "$9", $10, $13, $14, $15, $16, $17, $18, $19, $20);.
    /// </remarks>
    public override void Initialize()
    {
        this.CreateScroll(0, 1, LocalizedString.FromResource(() => ItemNames.ScrollOfPoison), 30, 0, 140, 17000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(1, 2, LocalizedString.FromResource(() => ItemNames.ScrollOfMeteorite), 21, 0, 104, 11000, 1, 0, 0, 1, 0, 1, 0);
        this.CreateScroll(2, 3, LocalizedString.FromResource(() => ItemNames.ScrollOfLighting), 13, 0, 72, 3000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(3, 4, LocalizedString.FromResource(() => ItemNames.ScrollOfFireBall), 5, 0, 40, 300, 1, 0, 0, 1, 0, 1, 0);
        this.CreateScroll(4, 5, LocalizedString.FromResource(() => ItemNames.ScrollOfFlame), 35, 0, 160, 21000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(5, 6, LocalizedString.FromResource(() => ItemNames.ScrollOfTeleport), 17, 0, 88, 5000, 1, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(6, 7, LocalizedString.FromResource(() => ItemNames.ScrollOfIce), 25, 0, 120, 14000, 1, 0, 0, 1, 0, 1, 0);
        this.CreateScroll(7, 8, LocalizedString.FromResource(() => ItemNames.ScrollOfTwister), 40, 0, 180, 25000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(8, 9, LocalizedString.FromResource(() => ItemNames.ScrollOfEvilSpirit), 50, 0, 220, 35000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(9, 10, LocalizedString.FromResource(() => ItemNames.ScrollOfHellfire), 60, 0, 260, 60000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(10, 11, LocalizedString.FromResource(() => ItemNames.ScrollOfPowerWave), 9, 0, 56, 1100, 1, 0, 0, 1, 0, 1, 0);
        this.CreateScroll(11, 12, LocalizedString.FromResource(() => ItemNames.ScrollOfAquaBeam), 74, 0, 345, 100000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(12, 13, LocalizedString.FromResource(() => ItemNames.ScrollOfCometfall), 80, 0, 436, 175000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(13, 14, LocalizedString.FromResource(() => ItemNames.ScrollOfInferno), 88, 0, 578, 265000, 1, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(14, 15, LocalizedString.FromResource(() => ItemNames.ScrollOfTeleportAlly), 83, 0, 644, 245000, 2, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(15, 16, LocalizedString.FromResource(() => ItemNames.ScrollOfSoulBarrier), 77, 0, 408, 135000, 1, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(16, 38, LocalizedString.FromResource(() => ItemNames.ScrollOfDecay), 96, 0, 953, 345000, 2, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(17, 39, LocalizedString.FromResource(() => ItemNames.ScrollOfIceStorm), 93, 0, 849, 315000, 2, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(18, 40, LocalizedString.FromResource(() => ItemNames.ScrollOfNova), 100, 0, 1052, 410000, 2, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(19, 215, LocalizedString.FromResource(() => ItemNames.ChainLightningParchment), 75, 0, 245, 175000, 0, 0, 0, 0, 0, 1, 0);
        this.CreateScroll(20, 214, LocalizedString.FromResource(() => ItemNames.DrainLifeParchment), 35, 0, 150, 100000, 0, 0, 0, 0, 0, 1, 0);
        this.CreateScroll(21, 230, LocalizedString.FromResource(() => ItemNames.LightningShockParchment), 93, 0, 823, 315000, 0, 0, 0, 0, 0, 1, 0);
        this.CreateScroll(22, 217, LocalizedString.FromResource(() => ItemNames.DamageReflectionParchment), 80, 0, 375, 245000, 0, 0, 0, 0, 0, 1, 0);
        this.CreateScroll(23, 218, LocalizedString.FromResource(() => ItemNames.BerserkerParchment), 83, 0, 620, 265000, 0, 0, 0, 0, 0, 1, 0);
        this.CreateScroll(24, 219, LocalizedString.FromResource(() => ItemNames.SleepParchment), 40, 0, 180, 135000, 0, 0, 0, 0, 0, 1, 0);
        this.CreateScroll(26, 221, LocalizedString.FromResource(() => ItemNames.WeaknessParchment), 93, 0, 663, 410000, 0, 0, 0, 0, 0, 2, 0);
        this.CreateScroll(27, 222, LocalizedString.FromResource(() => ItemNames.InnovationParchment), 111, 0, 912, 450000, 0, 0, 0, 0, 0, 2, 0);
        this.CreateScroll(28, 233, LocalizedString.FromResource(() => ItemNames.ScrollOfWizardryEnhance), 100, 220, 118, 425000, 2, 0, 0, 0, 0, 0, 0);
        this.CreateScroll(29, 237, LocalizedString.FromResource(() => ItemNames.ScrollOfGiganticStorm), 100, 220, 118, 380000, 0, 0, 0, 1, 0, 0, 0);
        this.CreateScroll(30, 262, LocalizedString.FromResource(() => ItemNames.ChainDriveParchment), 80, 150, 0, 175000, 0, 0, 0, 0, 0, 0, 1);
        this.CreateScroll(31, 263, LocalizedString.FromResource(() => ItemNames.DarkSideParchment), 100, 180, 0, 345000, 0, 0, 0, 0, 0, 0, 1);
        this.CreateScroll(32, 264, LocalizedString.FromResource(() => ItemNames.DragonRoarParchment), 90, 150, 0, 265000, 0, 0, 0, 0, 0, 0, 1);
        this.CreateScroll(33, 265, LocalizedString.FromResource(() => ItemNames.DragonSlasherParchment), 100, 200, 0, 345000, 0, 0, 0, 0, 0, 0, 1);
        this.CreateScroll(34, 266, LocalizedString.FromResource(() => ItemNames.IgnoreDefenseParchment), 100, 120, 404, 345000, 0, 0, 0, 0, 0, 0, 1);
        this.CreateScroll(35, 267, LocalizedString.FromResource(() => ItemNames.IncreaseHealthParchment), 90, 80, 132, 265000, 0, 0, 0, 0, 0, 0, 1);
        this.CreateScroll(36, 268, LocalizedString.FromResource(() => ItemNames.IncreaseBlockParchment), 70, 50, 80, 60000, 0, 0, 0, 0, 0, 0, 1);
    }

    private void CreateScroll(byte number, int skillNumber, LocalizedString name, byte dropLevel, int levelRequirement, int energyRequirement, int money, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
    {
        var scroll = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(scroll);
        scroll.Group = 15;
        scroll.Number = number;
        scroll.Skill = this.GameConfiguration.Skills.First(skill => skill.Number == skillNumber);
        scroll.Width = 1;
        scroll.Height = 2;
        scroll.Name = name;
        scroll.DropLevel = dropLevel;
        scroll.DropsFromMonsters = true;
        scroll.Durability = 1;

        this.CreateItemRequirementIfNeeded(scroll, Stats.Level, levelRequirement);
        this.CreateItemRequirementIfNeeded(scroll, Stats.TotalEnergyRequirementValue, energyRequirement);

        scroll.Value = money;
        scroll.SetGuid(scroll.Group, scroll.Number);
        var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
        foreach (var characterClass in classes)
        {
            scroll.QualifiedCharacters.Add(characterClass);
        }
    }
}