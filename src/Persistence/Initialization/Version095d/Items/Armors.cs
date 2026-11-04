// <copyright file="Armors.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.DataModel.Configuration;
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
    protected override byte MaximumArmorLevel => Constants.MaximumItemLevel;

    /// <summary>
    /// Initializes armor data.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        // Shields:
        this.CreateShield(0, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.SmallShield), 3, 1, 3, 22, 70, 0, 1, 1, 1);
        this.CreateShield(1, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.HornShield), 9, 3, 9, 28, 100, 0, 0, 1, 0);
        this.CreateShield(2, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.KiteShield), 12, 4, 12, 32, 110, 0, 0, 1, 0);
        this.CreateShield(3, 1, 0, 2, 2, LocalizedString.FromResource(() => ItemNames.ElvenShield), 21, 8, 21, 36, 30, 100, 0, 0, 1);
        this.CreateShield(4, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.Buckler), 6, 2, 6, 24, 80, 0, 1, 1, 1);
        this.CreateShield(5, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonSlayerShield), 35, 10, 36, 44, 100, 40, 0, 1, 0);
        this.CreateShield(6, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SkullShield), 15, 5, 15, 34, 110, 0, 1, 1, 1);
        this.CreateShield(7, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SpikedShield), 30, 9, 30, 40, 130, 0, 0, 1, 0);
        this.CreateShield(8, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.TowerShield), 40, 11, 40, 46, 130, 0, 0, 1, 1);
        this.CreateShield(9, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateShield), 25, 8, 25, 38, 120, 0, 0, 1, 0);
        this.CreateShield(10, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.BigRoundShield), 18, 6, 18, 35, 120, 0, 0, 1, 0);
        this.CreateShield(11, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.SerpentShield), 45, 12, 45, 48, 130, 0, 0, 1, 1);
        this.CreateShield(12, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeShield), 54, 13, 54, 52, 140, 0, 0, 1, 0);
        this.CreateShield(13, 1, 18, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonShield), 60, 14, 60, 60, 120, 40, 0, 1, 0);
        this.CreateShield(14, 1, 0, 2, 3, LocalizedString.FromResource(() => ItemNames.LegendaryShield), 48, 7, 48, 50, 90, 25, 1, 0, 1);

        // Helmets:
        this.CreateArmor(0, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeHelm), 16, 9, 34, 80, 20, 0, 1, 0);
        this.CreateArmor(1, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonHelm), 57, 24, 68, 120, 30, 0, 1, 0);
        this.CreateArmor(2, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PadHelm), 5, 4, 28, 20, 0, 1, 0, 0);
        this.CreateArmor(3, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryHelm), 50, 18, 42, 30, 0, 1, 0, 0);
        this.CreateArmor(4, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BoneHelm), 18, 9, 30, 30, 0, 1, 0, 0);
        this.CreateArmor(5, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.LeatherHelm), 6, 5, 30, 80, 0, 0, 1, 0);
        this.CreateArmor(6, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleHelm), 26, 12, 40, 110, 0, 0, 1, 0);
        this.CreateArmor(7, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxMask), 32, 13, 36, 30, 0, 1, 0, 0);
        this.CreateArmor(8, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassHelm), 36, 17, 44, 100, 30, 0, 1, 0);
        this.CreateArmor(9, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateHelm), 46, 20, 50, 130, 0, 0, 1, 0);
        this.CreateArmor(10, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.VineHelm), 6, 4, 22, 30, 60, 0, 0, 1);
        this.CreateArmor(11, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkHelm), 16, 8, 26, 30, 70, 0, 0, 1);
        this.CreateArmor(12, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.WindHelm), 28, 12, 32, 30, 80, 0, 0, 1);
        this.CreateArmor(13, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritHelm), 40, 16, 38, 40, 80, 0, 0, 1);
        this.CreateArmor(14, 2, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianHelm), 53, 23, 45, 40, 80, 0, 0, 1);

        // Armors:
        this.CreateArmor(0, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeArmor), 18, 14, 34, 80, 20, 0, 1, 0);
        this.CreateArmor(1, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.DragonArmor), 59, 37, 68, 120, 30, 0, 1, 0);
        this.CreateArmor(2, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.PadArmor), 10, 7, 28, 30, 0, 1, 0, 0);
        this.CreateArmor(3, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryArmor), 56, 22, 42, 40, 0, 1, 0, 0);
        this.CreateArmor(4, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BoneArmor), 22, 13, 30, 40, 0, 1, 0, 0);
        this.CreateArmor(5, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.LeatherArmor), 10, 10, 30, 80, 0, 0, 1, 0);
        this.CreateArmor(6, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleArmor), 28, 18, 40, 110, 0, 0, 1, 0);
        this.CreateArmor(7, 3, 2, 3, LocalizedString.FromResource(() => ItemNames.SphinxArmor), 38, 17, 36, 40, 0, 1, 0, 0);
        this.CreateArmor(8, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassArmor), 38, 22, 44, 100, 30, 0, 1, 0);
        this.CreateArmor(9, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateArmor), 48, 30, 50, 130, 0, 0, 1, 0);
        this.CreateArmor(10, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.VineArmor), 10, 8, 22, 30, 60, 0, 0, 1);
        this.CreateArmor(11, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkArmor), 20, 12, 26, 30, 70, 0, 0, 1);
        this.CreateArmor(12, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.WindArmor), 32, 16, 32, 30, 80, 0, 0, 1);
        this.CreateArmor(13, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritArmor), 44, 21, 38, 40, 80, 0, 0, 1);
        this.CreateArmor(14, 3, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianArmor), 57, 29, 45, 40, 80, 0, 0, 1);

        // Pants:
        this.CreateArmor(0, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzePants), 15, 10, 34, 80, 20, 0, 1, 0);
        this.CreateArmor(1, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonPants), 55, 26, 68, 120, 30, 0, 1, 0);
        this.CreateArmor(2, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PadPants), 8, 5, 28, 30, 0, 1, 0, 0);
        this.CreateArmor(3, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryPants), 53, 20, 42, 40, 0, 1, 0, 0);
        this.CreateArmor(4, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BonePants), 20, 10, 30, 40, 0, 1, 0, 0);
        this.CreateArmor(5, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.LeatherPants), 8, 7, 30, 80, 0, 0, 1, 0);
        this.CreateArmor(6, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.ScalePants), 25, 14, 40, 110, 0, 0, 1, 0);
        this.CreateArmor(7, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxPants), 34, 15, 36, 40, 0, 1, 0, 0);
        this.CreateArmor(8, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassPants), 35, 18, 44, 100, 30, 0, 1, 0);
        this.CreateArmor(9, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.PlatePants), 45, 22, 50, 130, 0, 0, 1, 0);
        this.CreateArmor(10, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.VinePants), 8, 6, 22, 30, 60, 0, 0, 1);
        this.CreateArmor(11, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkPants), 18, 10, 26, 30, 70, 0, 0, 1);
        this.CreateArmor(12, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.WindPants), 30, 14, 32, 30, 80, 0, 0, 1);
        this.CreateArmor(13, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritPants), 42, 18, 38, 40, 80, 0, 0, 1);
        this.CreateArmor(14, 4, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianPants), 54, 25, 45, 40, 80, 0, 0, 1);

        // Gloves:
        this.CreateGloves(0, LocalizedString.FromResource(() => ItemNames.BronzeGloves), 13, 4, 4, 34, 80, 20, 0, 1, 0);
        this.CreateGloves(1, LocalizedString.FromResource(() => ItemNames.DragonGloves), 52, 14, 6, 68, 120, 30, 0, 1, 0);
        this.CreateGloves(2, LocalizedString.FromResource(() => ItemNames.PadGloves), 3, 2, 0, 28, 20, 0, 1, 0, 0);
        this.CreateGloves(3, LocalizedString.FromResource(() => ItemNames.LegendaryGloves), 44, 11, 0, 42, 20, 0, 1, 0, 0);
        this.CreateGloves(4, LocalizedString.FromResource(() => ItemNames.BoneGloves), 14, 5, 0, 30, 20, 0, 1, 0, 0);
        this.CreateGloves(5, LocalizedString.FromResource(() => ItemNames.LeatherGloves), 4, 2, 8, 30, 80, 0, 0, 1, 0);
        this.CreateGloves(6, LocalizedString.FromResource(() => ItemNames.ScaleGloves), 22, 7, 10, 40, 110, 0, 0, 1, 0);
        this.CreateGloves(7, LocalizedString.FromResource(() => ItemNames.SphinxGloves), 28, 8, 0, 36, 20, 0, 1, 0, 0);
        this.CreateGloves(8, LocalizedString.FromResource(() => ItemNames.BrassGloves), 32, 9, 8, 44, 100, 30, 0, 1, 0);
        this.CreateGloves(9, LocalizedString.FromResource(() => ItemNames.PlateGloves), 42, 12, 4, 50, 130, 0, 0, 1, 0);
        this.CreateGloves(10, LocalizedString.FromResource(() => ItemNames.VineGloves), 4, 2, 4, 22, 30, 60, 0, 0, 1);
        this.CreateGloves(11, LocalizedString.FromResource(() => ItemNames.SilkGloves), 14, 4, 8, 26, 30, 70, 0, 0, 1);
        this.CreateGloves(12, LocalizedString.FromResource(() => ItemNames.WindGloves), 26, 6, 10, 32, 30, 80, 0, 0, 1);
        this.CreateGloves(13, LocalizedString.FromResource(() => ItemNames.SpiritGloves), 38, 9, 4, 38, 40, 80, 0, 0, 1);
        this.CreateGloves(14, LocalizedString.FromResource(() => ItemNames.GuardianGloves), 50, 15, 6, 45, 40, 80, 0, 0, 1);

        // Boots:
        this.CreateBoots(0, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.BronzeBoots), 12, 4, 10, 34, 80, 20, 0, 1, 0);
        this.CreateBoots(1, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonBoots), 54, 15, 2, 68, 120, 30, 0, 1, 0);
        this.CreateBoots(2, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.PadBoots), 4, 3, 10, 28, 20, 0, 1, 0, 0);
        this.CreateBoots(3, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryBoots), 46, 12, 0, 42, 30, 0, 1, 0, 0);
        this.CreateBoots(4, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.BoneBoots), 16, 6, 6, 30, 30, 0, 1, 0, 0);
        this.CreateBoots(5, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.LeatherBoots), 5, 2, 12, 30, 80, 0, 0, 1, 0);
        this.CreateBoots(6, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleBoots), 22, 8, 8, 40, 110, 0, 0, 1, 0);
        this.CreateBoots(7, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxBoots), 30, 9, 8, 36, 30, 0, 1, 0, 0);
        this.CreateBoots(8, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.BrassBoots), 32, 10, 6, 44, 100, 30, 0, 1, 0);
        this.CreateBoots(9, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.PlateBoots), 42, 12, 4, 50, 130, 0, 0, 1, 0);
        this.CreateBoots(10, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.VineBoots), 5, 2, 0, 22, 30, 60, 0, 0, 1);
        this.CreateBoots(11, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkBoots), 15, 4, 0, 26, 30, 70, 0, 0, 1);
        this.CreateBoots(12, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.WindBoots), 27, 7, 0, 32, 30, 80, 0, 0, 1);
        this.CreateBoots(13, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.SpiritBoots), 40, 10, 0, 38, 40, 80, 0, 0, 1);
        this.CreateBoots(14, 6, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianBoots), 52, 16, 0, 45, 40, 80, 0, 0, 1);

        this.BuildSets();
    }
}