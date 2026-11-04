// <copyright file="Armors.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initializer for armor data.
/// </summary>
public class Armors : ArmorInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Armors"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Armors(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override byte MaximumArmorLevel => 15;

    /// <summary>
    /// Initializes armor data.
    /// </summary>
    /// <remarks>
    /// Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
    /// Replace by: this.CreateArmor($1, $2, $4, $5, "$9", $10, $11, $13, $14, $15, $16, $17, $18, $19, $21, $22, $23, $24, $25, $26, $27);.
    /// </remarks>
    public override void Initialize()
    {
        base.Initialize();

        // Shields:
        this.CreateShield(0, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.SmallShield), 3, 1, 3, 22, 0, 70, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateShield(1, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.HornShield), 9, 3, 9, 28, 0, 100, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(2, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.KiteShield), 12, 4, 12, 32, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(3, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.ElvenShield), 21, 8, 21, 36, 0, 30, 100, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateShield(4, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.Buckler), 6, 2, 6, 24, 0, 80, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateShield(5, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonSlayerShield), 35, 10, 36, 44, 0, 100, 40, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(6, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SkullShield), 15, 5, 15, 34, 0, 110, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateShield(7, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SpikedShield), 30, 9, 30, 40, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(8, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.TowerShield), 40, 11, 40, 46, 0, 130, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0);
        this.CreateShield(9, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateShield), 25, 8, 25, 38, 0, 120, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(10, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.BigRoundShield), 18, 6, 18, 35, 0, 120, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(11, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SerpentShield), 45, 12, 45, 48, 0, 130, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0);
        this.CreateShield(12, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeShield), 54, 13, 54, 52, 0, 140, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateShield(13, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonShield), 60, 14, 60, 60, 0, 120, 40, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateShield(14, 1, 0, 2, 3, LocalizedString.FromResource(() => ItemNames.LegendaryShield), 48, 7, 48, 50, 0, 90, 25, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0);
        this.CreateShield(15, 1, 0, 2, 3, LocalizedString.FromResource(() => ItemNames.GrandSoulShield), 74, 12, 55, 55, 0, 70, 23, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateShield(16, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.ElementalShield), 66, 11, 28, 51, 0, 30, 60, 30, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateShield(17, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.CrimsonGlory), 104, 19, 90, 51, 0, 95, 48, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateShield(18, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SalamanderShield), 102, 20, 96, 51, 0, 80, 61, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateShield(19, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.FrostBarrier), 99, 14, 58, 51, 0, 26, 53, 26, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateShield(20, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianShield), 106, 12, 30, 51, 0, 54, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateShield(21, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.CrossShield), 70, 16, 75, 65, 0, 140, 55, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);

        // Helmets:
        this.CreateArmor(0, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeHelm), 16, 9, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0);
        this.CreateArmor(1, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonHelm), 57, 24, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(2, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PadHelm), 5, 4, 28, 0, 20, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(3, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryHelm), 50, 18, 42, 0, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(4, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BoneHelm), 18, 9, 30, 0, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(5, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.LeatherHelm), 6, 5, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1);
        this.CreateArmor(6, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleHelm), 26, 12, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1);
        this.CreateArmor(7, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxMask), 32, 13, 36, 0, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(8, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassHelm), 36, 17, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        this.CreateArmor(9, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateHelm), 46, 20, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        this.CreateArmor(10, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.VineHelm), 6, 4, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkHelm), 16, 8, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.WindHelm), 28, 12, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritHelm), 40, 16, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianHelm), 53, 23, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(16, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BlackDragonHelm), 82, 30, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkPhoenixHelm), 92, 43, 80, 0, 205, 62, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.GrandSoulHelm), 81, 27, 67, 0, 59, 20, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DivineHelm), 85, 37, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(21, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.GreatDragonHelm), 104, 53, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkSoulHelm), 110, 36, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(24, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.RedSpiritHelm), 93, 46, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.LightPlateMask), 46, 20, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.AdamantineMask), 66, 24, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkSteelMask), 86, 26, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkMasterMask), 106, 34, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonKnightHelm), 130, 66, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.VenomMistHelm), 126, 48, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SylphidRayHelm), 126, 57, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(33, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SunlightMask), 130, 46, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.AshcrowHelm), 67, 27, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.EclipseHelm), 67, 22, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.IrisHelm), 67, 30, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(38, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.GloriousMask), 97, 30, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.MisteryHelm), 28, 13, 36, 0, 31, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingHelm), 50, 18, 42, 0, 26, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.AncientHelm), 68, 24, 54, 0, 52, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BlackRoseHelm), 81, 32, 67, 0, 60, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.AuraHelm), 110, 43, 75, 380, 56, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.LiliumHelm), 110, 50, 80, 0, 80, 50, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.TitanHelm), 111, 63, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BraveHelm), 107, 51, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(49, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SeraphimHelm), 111, 50, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.FaithHelm), 104, 44, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PaewangMask), 111, 44, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.HadesHelm), 109, 41, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SacredHelm), 54, 24, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.StormHardHelm), 70, 32, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PiercingHelm), 90, 45, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PhoenixSoulHelmet), 128, 60, 88, 380, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        // Armors:
        this.CreateArmor(0, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeArmor), 18, 14, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(1, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DragonArmor), 59, 37, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(2, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.PadArmor), 10, 7, 28, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(3, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryArmor), 56, 22, 42, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(4, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BoneArmor), 22, 13, 30, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(5, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.LeatherArmor), 10, 10, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(6, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleArmor), 28, 18, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(7, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.SphinxArmor), 38, 17, 36, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(8, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassArmor), 38, 22, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(9, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateArmor), 48, 30, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(10, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.VineArmor), 10, 8, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkArmor), 20, 12, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.WindArmor), 32, 16, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritArmor), 44, 21, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianArmor), 57, 29, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(15, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.StormCrowArmor), 80, 44, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(16, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.BlackDragonArmor), 90, 48, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DarkPhoenixArmor), 100, 63, 80, 0, 214, 65, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.GrandSoulArmor), 91, 33, 67, 0, 59, 20, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.DivineArmor), 92, 44, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(20, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.ThunderHawkArmor), 107, 60, 82, 0, 170, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(21, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.GreatDragonArmor), 126, 75, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DarkSoulArmor), 122, 43, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(23, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.HurricaneArmor), 128, 73, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(24, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.RedSpritArmor), 109, 55, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.LightPlateArmor), 62, 25, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.AdamantineArmor), 78, 36, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DarkSteelArmor), 96, 43, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DarkMasterArmor), 117, 51, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DragonKnightArmor), 140, 88, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.VenomMistArmor), 146, 57, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.SylphidRayArmor), 146, 68, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(32, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.VolcanoArmor), 147, 86, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(33, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.SunlightArmor), 147, 64, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.AshcrowArmor), 75, 42, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.EclipseArmor), 75, 27, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.IrisArmor), 75, 36, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(37, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.ValiantArmor), 105, 52, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(38, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.GloriousArmor), 105, 47, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.MisteryArmor), 34, 22, 36, 0, 39, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingArmor), 56, 28, 42, 0, 35, 8, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.AncientArmor), 75, 35, 54, 0, 52, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BlackRoseArmor), 91, 45, 67, 0, 60, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.AuraArmor), 122, 56, 75, 380, 57, 19, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.LiliumArmor), 113, 71, 84, 0, 110, 50, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.TitanArmor), 132, 81, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.BraveArmor), 128, 62, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(47, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DestoryArmor), 131, 80, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(48, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.PhantomArmor), 125, 66, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(49, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.SeraphimArmor), 129, 60, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.FaithArmor), 122, 52, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.PaewangArmor), 132, 58, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.HadesArmor), 129, 50, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.SacredArmor), 66, 43, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.StormHardArmor), 82, 51, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.PiercingArmor), 101, 59, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.PhoenixSoulArmor), 143, 78, 88, 380, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        // Pants:
        this.CreateArmor(0, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzePants), 15, 10, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(1, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonPants), 55, 26, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(2, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PadPants), 8, 5, 28, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(3, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryPants), 53, 20, 42, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(4, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BonePants), 20, 10, 30, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(5, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.LeatherPants), 8, 7, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(6, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.ScalePants), 25, 14, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(7, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxPants), 34, 15, 36, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(8, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassPants), 35, 18, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(9, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PlatePants), 45, 22, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(10, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.VinePants), 8, 6, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkPants), 18, 10, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.WindPants), 30, 14, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritPants), 42, 18, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianPants), 54, 25, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(15, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.StormCrowPants), 74, 34, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(16, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BlackDragonPants), 84, 40, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkPhoenixPants), 96, 54, 80, 0, 207, 63, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.GrandSoulPants), 86, 30, 67, 0, 59, 20, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DivinePants), 88, 39, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(20, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.ThunderHawkPants), 99, 49, 82, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(21, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.GreatDragonPants), 113, 65, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkSoulPants), 117, 39, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(23, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.HurricanePants), 122, 61, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(24, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.RedSpiritPants), 100, 48, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.LightPlatePants), 50, 21, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.AdamantinePants), 70, 26, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkSteelPants), 92, 31, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DarkMasterPants), 110, 39, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonKnightPants), 134, 78, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.VenomMistPants), 135, 55, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SylphidRayPants), 135, 61, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(32, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.VolcanoPants), 135, 74, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(33, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SunlightPants), 140, 52, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.AshcrowPants), 69, 33, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.EclipsePants), 69, 25, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.IrisPants), 69, 32, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(37, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.ValiantPants), 101, 41, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(38, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.GloriousPants), 101, 35, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.MisteryPants), 30, 16, 36, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingPants), 53, 22, 42, 0, 35, 7, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.AncientPants), 72, 28, 54, 0, 49, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BlackRosePants), 86, 37, 67, 0, 60, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.AuraPants), 117, 49, 75, 380, 57, 19, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.LiliumPants), 102, 52, 82, 0, 75, 30, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.TitanPants), 116, 74, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BravePants), 112, 58, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(47, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DestoryPants), 115, 66, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(48, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PhantomPants), 113, 51, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(49, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SeraphimPants), 116, 53, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.FaithPants), 109, 46, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PaewangPants), 116, 46, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.HadesPants), 114, 44, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SacredPants), 62, 33, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.StormHardPants), 78, 41, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PiercingPants), 95, 49, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PhoenixSoulPants), 134, 68, 88, 380, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        // Gloves:
        this.CreateGloves(0, LocalizedString.FromResource(() => ItemNames.BronzeGloves), 13, 4, 4, 34, 0, 80, 20, 0, 1, 0, 1, 1, 0);
        this.CreateGloves(1, LocalizedString.FromResource(() => ItemNames.DragonGloves), 52, 14, 6, 68, 0, 120, 30, 0, 1, 0, 1, 0, 0);
        this.CreateGloves(2, LocalizedString.FromResource(() => ItemNames.PadGloves), 3, 2, 0, 28, 0, 20, 0, 1, 0, 0, 1, 0, 0);
        this.CreateGloves(3, LocalizedString.FromResource(() => ItemNames.LegendaryGloves), 44, 11, 0, 42, 0, 20, 0, 1, 0, 0, 1, 0, 0);
        this.CreateGloves(4, LocalizedString.FromResource(() => ItemNames.BoneGloves), 14, 5, 0, 30, 0, 20, 0, 1, 0, 0, 1, 0, 0);
        this.CreateGloves(5, LocalizedString.FromResource(() => ItemNames.LeatherGloves), 4, 2, 8, 30, 0, 80, 0, 0, 1, 0, 1, 1, 0);
        this.CreateGloves(6, LocalizedString.FromResource(() => ItemNames.ScaleGloves), 22, 7, 10, 40, 0, 110, 0, 0, 1, 0, 1, 1, 0);
        this.CreateGloves(7, LocalizedString.FromResource(() => ItemNames.SphinxGloves), 28, 8, 0, 36, 0, 20, 0, 1, 0, 0, 1, 0, 0);
        this.CreateGloves(8, LocalizedString.FromResource(() => ItemNames.BrassGloves), 32, 9, 8, 44, 0, 100, 30, 0, 1, 0, 1, 0, 0);
        this.CreateGloves(9, LocalizedString.FromResource(() => ItemNames.PlateGloves), 42, 12, 4, 50, 0, 130, 0, 0, 1, 0, 1, 0, 0);
        this.CreateGloves(10, LocalizedString.FromResource(() => ItemNames.VineGloves), 4, 2, 4, 22, 0, 30, 60, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(11, LocalizedString.FromResource(() => ItemNames.SilkGloves), 14, 4, 8, 26, 0, 30, 70, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(12, LocalizedString.FromResource(() => ItemNames.WindGloves), 26, 6, 10, 32, 0, 30, 80, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(13, LocalizedString.FromResource(() => ItemNames.SpiritGloves), 38, 9, 4, 38, 0, 40, 80, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(14, LocalizedString.FromResource(() => ItemNames.GuardianGloves), 50, 15, 6, 45, 0, 40, 80, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(15, LocalizedString.FromResource(() => ItemNames.StormCrowGloves), 70, 20, 6, 80, 0, 150, 70, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(16, LocalizedString.FromResource(() => ItemNames.BlackDragonGloves), 76, 22, 6, 74, 0, 170, 60, 0, 1, 0, 0, 0, 0);
        this.CreateGloves(17, LocalizedString.FromResource(() => ItemNames.DarkPhoenixGloves), 86, 37, 6, 80, 0, 205, 63, 0, 2, 0, 0, 0, 0);
        this.CreateGloves(18, LocalizedString.FromResource(() => ItemNames.GrandSoulGloves), 70, 20, 5, 67, 0, 49, 10, 2, 0, 0, 0, 0, 0);
        this.CreateGloves(19, LocalizedString.FromResource(() => ItemNames.DivineGloves), 72, 29, 6, 74, 0, 50, 110, 0, 0, 2, 0, 0, 0);
        this.CreateGloves(20, LocalizedString.FromResource(() => ItemNames.ThunderHawkGloves), 88, 34, 7, 82, 0, 150, 70, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(21, LocalizedString.FromResource(() => ItemNames.GreatDragonGloves), 94, 48, 6, 86, 0, 200, 58, 0, 2, 0, 0, 0, 0);
        this.CreateGloves(22, LocalizedString.FromResource(() => ItemNames.DarkSoulGloves), 87, 30, 6, 75, 0, 55, 18, 2, 0, 0, 0, 0, 0);
        this.CreateGloves(23, LocalizedString.FromResource(() => ItemNames.HurricaneGloves), 102, 45, 7, 90, 0, 162, 66, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(24, LocalizedString.FromResource(() => ItemNames.RedSpiritGloves), 84, 38, 6, 80, 0, 52, 115, 0, 0, 2, 0, 0, 0);
        this.CreateGloves(25, LocalizedString.FromResource(() => ItemNames.LightPlateGloves), 42, 12, 7, 42, 0, 70, 20, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(26, LocalizedString.FromResource(() => ItemNames.AdamantineGloves), 57, 18, 6, 56, 0, 77, 21, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(27, LocalizedString.FromResource(() => ItemNames.DarkSteelGloves), 75, 21, 5, 70, 0, 84, 22, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(28, LocalizedString.FromResource(() => ItemNames.DarkMasterGloves), 89, 29, 4, 78, 0, 80, 21, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(29, LocalizedString.FromResource(() => ItemNames.DragonKnightGloves), 114, 60, 7, 90, 380, 170, 60, 0, 2, 0, 0, 0, 0);
        this.CreateGloves(30, LocalizedString.FromResource(() => ItemNames.VenomMistGloves), 111, 44, 7, 86, 380, 44, 15, 2, 0, 0, 0, 0, 0);
        this.CreateGloves(31, LocalizedString.FromResource(() => ItemNames.SylphidRayGloves), 111, 50, 7, 86, 380, 38, 80, 0, 0, 2, 0, 0, 0);
        this.CreateGloves(32, LocalizedString.FromResource(() => ItemNames.VolcanoGloves), 127, 55, 7, 95, 380, 145, 60, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(33, LocalizedString.FromResource(() => ItemNames.SunlightGloves), 110, 40, 5, 82, 380, 62, 16, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(34, LocalizedString.FromResource(() => ItemNames.AshcrowGloves), 61, 18, 6, 72, 0, 160, 50, 0, 1, 0, 0, 0, 0);
        this.CreateGloves(35, LocalizedString.FromResource(() => ItemNames.EclipseGloves), 61, 15, 6, 54, 0, 53, 12, 1, 0, 0, 0, 0, 0);
        this.CreateGloves(36, LocalizedString.FromResource(() => ItemNames.IrisGloves), 61, 22, 6, 59, 0, 50, 70, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(37, LocalizedString.FromResource(() => ItemNames.ValiantGloves), 91, 27, 7, 81, 0, 155, 50, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(38, LocalizedString.FromResource(() => ItemNames.GloriousGloves), 91, 25, 5, 74, 0, 80, 21, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(39, LocalizedString.FromResource(() => ItemNames.MisteryGloves), 24, 9, 6, 36, 0, 22, 0, 0, 0, 0, 0, 0, 1);
        this.CreateGloves(40, LocalizedString.FromResource(() => ItemNames.RedWingGloves), 44, 13, 8, 42, 0, 18, 4, 0, 0, 0, 0, 0, 1);
        this.CreateGloves(41, LocalizedString.FromResource(() => ItemNames.AncientGloves), 61, 19, 7, 54, 0, 52, 16, 0, 0, 0, 0, 0, 1);
        this.CreateGloves(42, LocalizedString.FromResource(() => ItemNames.BlackRoseGloves), 70, 26, 6, 67, 0, 50, 10, 0, 0, 0, 0, 0, 2);
        this.CreateGloves(43, LocalizedString.FromResource(() => ItemNames.AuraGloves), 87, 34, 6, 75, 380, 56, 20, 0, 0, 0, 0, 0, 2);
        this.CreateGloves(44, LocalizedString.FromResource(() => ItemNames.LiliumGloves), 82, 45, 6, 80, 0, 75, 20, 0, 0, 0, 0, 0, 2);
        this.CreateGloves(45, LocalizedString.FromResource(() => ItemNames.TitanGloves), 100, 56, 7, 86, 0, 222, 32, 0, 1, 0, 0, 0, 0);
        this.CreateGloves(46, LocalizedString.FromResource(() => ItemNames.BraveGloves), 97, 42, 7, 86, 0, 74, 162, 0, 1, 0, 0, 0, 0);
        this.CreateGloves(47, LocalizedString.FromResource(() => ItemNames.DestroyGloves), 101, 49, 7, 86, 0, 212, 57, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(48, LocalizedString.FromResource(() => ItemNames.PhantomGloves), 99, 40, 7, 86, 0, 62, 19, 0, 0, 0, 1, 0, 0);
        this.CreateGloves(49, LocalizedString.FromResource(() => ItemNames.SeraphimGloves), 100, 43, 7, 86, 0, 55, 197, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(50, LocalizedString.FromResource(() => ItemNames.FaithGloves), 95, 36, 7, 86, 0, 32, 29, 0, 0, 1, 0, 0, 0);
        this.CreateGloves(51, LocalizedString.FromResource(() => ItemNames.PaewangGloves), 101, 34, 7, 86, 0, 105, 38, 0, 0, 0, 0, 1, 0);
        this.CreateGloves(52, LocalizedString.FromResource(() => ItemNames.HadesGloves), 100, 31, 7, 86, 0, 60, 15, 1, 0, 0, 0, 0, 0);

        // Boots:
        this.CreateBoots(0, LocalizedString.FromResource(() => ItemNames.BronzeBoots), 12, 4, 10, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateBoots(1, LocalizedString.FromResource(() => ItemNames.DragonBoots), 54, 15, 2, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateBoots(2, LocalizedString.FromResource(() => ItemNames.PadBoots), 4, 3, 10, 28, 0, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(3, LocalizedString.FromResource(() => ItemNames.LegendaryBoots), 46, 12, 0, 42, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(4, LocalizedString.FromResource(() => ItemNames.BoneBoots), 16, 6, 6, 30, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(5, LocalizedString.FromResource(() => ItemNames.LeatherBoots), 5, 2, 12, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateBoots(6, LocalizedString.FromResource(() => ItemNames.ScaleBoots), 22, 8, 8, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateBoots(7, LocalizedString.FromResource(() => ItemNames.SphinxBoots), 30, 9, 8, 36, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(8, LocalizedString.FromResource(() => ItemNames.BrassBoots), 32, 10, 6, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateBoots(9, LocalizedString.FromResource(() => ItemNames.PlateBoots), 42, 12, 4, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateBoots(10, LocalizedString.FromResource(() => ItemNames.VineBoots), 5, 2, 0, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(11, LocalizedString.FromResource(() => ItemNames.SilkBoots), 15, 4, 0, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(12, LocalizedString.FromResource(() => ItemNames.WindBoots), 27, 7, 0, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(13, LocalizedString.FromResource(() => ItemNames.SpiritBoots), 40, 10, 0, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(14, LocalizedString.FromResource(() => ItemNames.GuardianBoots), 52, 16, 0, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(15, LocalizedString.FromResource(() => ItemNames.StormCrowBoots), 72, 22, 2, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(16, LocalizedString.FromResource(() => ItemNames.BlackDragonBoots), 78, 24, 2, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateBoots(17, LocalizedString.FromResource(() => ItemNames.DarkPhoenixBoots), 93, 40, 2, 80, 0, 198, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateBoots(18, LocalizedString.FromResource(() => ItemNames.GrandSoulBoots), 76, 22, 0, 67, 0, 59, 10, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateBoots(19, LocalizedString.FromResource(() => ItemNames.DivineBoots), 81, 30, 0, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateBoots(20, LocalizedString.FromResource(() => ItemNames.ThunderHawkBoots), 92, 37, 2, 82, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(21, LocalizedString.FromResource(() => ItemNames.GreatDragonBoots), 98, 50, 0, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateBoots(22, LocalizedString.FromResource(() => ItemNames.DarkSoulBoots), 95, 31, 0, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateBoots(23, LocalizedString.FromResource(() => ItemNames.HurricaneBoots), 110, 50, 0, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(24, LocalizedString.FromResource(() => ItemNames.RedSpiritBoots), 87, 40, 0, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateBoots(25, LocalizedString.FromResource(() => ItemNames.LightPlateBoots), 45, 13, 0, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(26, LocalizedString.FromResource(() => ItemNames.AdamantineBoots), 60, 20, 0, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(27, LocalizedString.FromResource(() => ItemNames.DarkSteelBoots), 83, 25, 0, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(28, LocalizedString.FromResource(() => ItemNames.DarkMasterBoots), 95, 33, 0, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(29, LocalizedString.FromResource(() => ItemNames.DragonKnightBoots), 119, 63, 0, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateBoots(30, LocalizedString.FromResource(() => ItemNames.VenomMistBoots), 119, 47, 0, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateBoots(31, LocalizedString.FromResource(() => ItemNames.SylphidRayBoots), 119, 53, 0, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateBoots(32, LocalizedString.FromResource(() => ItemNames.VolcanoBoots), 131, 61, 0, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(33, LocalizedString.FromResource(() => ItemNames.SunlightBoots), 121, 44, 0, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(34, LocalizedString.FromResource(() => ItemNames.AshcrowBoots), 68, 19, 0, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateBoots(35, LocalizedString.FromResource(() => ItemNames.EclipseBoots), 68, 17, 0, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateBoots(36, LocalizedString.FromResource(() => ItemNames.IrisBoots), 68, 23, 0, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(37, LocalizedString.FromResource(() => ItemNames.ValiantBoots), 98, 29, 0, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(38, LocalizedString.FromResource(() => ItemNames.GloriousBoots), 98, 29, 0, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(39, LocalizedString.FromResource(() => ItemNames.MisteryBoots), 26, 11, 0, 36, 0, 27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateBoots(40, LocalizedString.FromResource(() => ItemNames.RedWingBoots), 46, 15, 0, 42, 0, 25, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateBoots(41, LocalizedString.FromResource(() => ItemNames.AncientBoots), 65, 21, 0, 54, 0, 53, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateBoots(42, LocalizedString.FromResource(() => ItemNames.BlackRoseBoots), 76, 28, 0, 67, 0, 60, 10, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateBoots(43, LocalizedString.FromResource(() => ItemNames.AuraBoots), 95, 38, 0, 75, 380, 57, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateBoots(44, LocalizedString.FromResource(() => ItemNames.LiliumBoots), 90, 50, 0, 85, 0, 150, 30, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateBoots(45, LocalizedString.FromResource(() => ItemNames.TitanBoots), 96, 57, 0, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateBoots(46, LocalizedString.FromResource(() => ItemNames.BraveBoots), 93, 45, 0, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateBoots(47, LocalizedString.FromResource(() => ItemNames.DestoryBoots), 97, 54, 0, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(48, LocalizedString.FromResource(() => ItemNames.PhantomBoots), 94, 44, 0, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateBoots(49, LocalizedString.FromResource(() => ItemNames.SeraphimBoots), 97, 42, 0, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(50, LocalizedString.FromResource(() => ItemNames.FaithBoots), 92, 35, 0, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateBoots(51, LocalizedString.FromResource(() => ItemNames.PhaewangBoots), 98, 38, 0, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateBoots(52, LocalizedString.FromResource(() => ItemNames.HadesBoots), 94, 34, 0, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateBoots(59, LocalizedString.FromResource(() => ItemNames.SacredBoots), 50, 20, 0, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateBoots(60, LocalizedString.FromResource(() => ItemNames.StormHardBoots), 62, 28, 0, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateBoots(61, LocalizedString.FromResource(() => ItemNames.PiercingBoots), 82, 36, 0, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateBoots(73, LocalizedString.FromResource(() => ItemNames.PhoenixSoulBoots), 119, 57, 0, 88, 380, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        this.BuildSets();
        this.AddGuardianOptionsToSets();
    }

    private void AddGuardianOptionsToSets()
    {
        const int dragonKnightSetIndex = 29;
        const int sunlightSetIndex = 33;
        for (int i = dragonKnightSetIndex; i <= sunlightSetIndex; i++)
        {
            this.AddGuardianOptionForArmor(i, ItemGroups.Armor);
            this.AddGuardianOptionForArmor(i, ItemGroups.Pants);
            this.AddGuardianOptionForArmor(i, ItemGroups.Helm);
            this.AddGuardianOptionForArmor(i, ItemGroups.Boots);
            this.AddGuardianOptionForArmor(i, ItemGroups.Gloves);
        }

        const int auraSetIndex = 43;
        this.AddGuardianOptionForArmor(auraSetIndex, ItemGroups.Armor);
        this.AddGuardianOptionForArmor(auraSetIndex, ItemGroups.Pants);
        this.AddGuardianOptionForArmor(auraSetIndex, ItemGroups.Helm);
        this.AddGuardianOptionForArmor(auraSetIndex, ItemGroups.Boots);
        this.AddGuardianOptionForArmor(auraSetIndex, ItemGroups.Gloves);
        const int phoenixSoulSetIndex = 73;
        this.AddGuardianOptionForArmor(phoenixSoulSetIndex, ItemGroups.Armor);
        this.AddGuardianOptionForArmor(phoenixSoulSetIndex, ItemGroups.Pants);
        this.AddGuardianOptionForArmor(phoenixSoulSetIndex, ItemGroups.Helm);
        this.AddGuardianOptionForArmor(phoenixSoulSetIndex, ItemGroups.Boots);
    }

    private void AddGuardianOptionForArmor(int setNumber, ItemGroups itemGroup)
    {
        var armor = this.GameConfiguration.Items.FirstOrDefault(item => item.Number == setNumber && item.Group == (int)itemGroup);
        if (armor is null)
        {
            return;
        }

        var itemOption = this.GameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.GuardianOption && po.Number == (int)itemGroup));
        armor.PossibleItemOptions.Add(itemOption);
    }
}