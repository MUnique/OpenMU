// <copyright file="Armors.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;

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
        this.CreateShield(0, 1, 0, 2, 2, "Small Shield", 3, 1, 3, 22, 0, 70, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateShield(1, 1, 0, 2, 2, "Horn Shield", 9, 3, 9, 28, 0, 100, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(2, 1, 0, 2, 2, "Kite Shield", 12, 4, 12, 32, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(3, 1, 0, 2, 2, "Elven Shield", 21, 8, 21, 36, 0, 30, 100, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateShield(4, 1, 18, 2, 2, "Buckler", 6, 2, 6, 24, 0, 80, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateShield(5, 1, 18, 2, 2, "Dragon Slayer Shield", 35, 10, 36, 44, 0, 100, 40, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(6, 1, 18, 2, 2, "Skull Shield", 15, 5, 15, 34, 0, 110, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateShield(7, 1, 18, 2, 2, "Spiked Shield", 30, 9, 30, 40, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(8, 1, 18, 2, 2, "Tower Shield", 40, 11, 40, 46, 0, 130, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0);
        this.CreateShield(9, 1, 18, 2, 2, "Plate Shield", 25, 8, 25, 38, 0, 120, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(10, 1, 18, 2, 2, "Big Round Shield", 18, 6, 18, 35, 0, 120, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateShield(11, 1, 18, 2, 2, "Serpent Shield", 45, 12, 45, 48, 0, 130, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0);
        this.CreateShield(12, 1, 18, 2, 2, "Bronze Shield", 54, 13, 54, 52, 0, 140, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateShield(13, 1, 18, 2, 2, "Dragon Shield", 60, 14, 60, 60, 0, 120, 40, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateShield(14, 1, 0, 2, 3, "Legendary Shield", 48, 7, 48, 50, 0, 90, 25, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0);
        this.CreateShield(15, 1, 0, 2, 3, "Grand Soul Shield", 74, 12, 55, 55, 0, 70, 23, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateShield(16, 1, 0, 2, 2, "Elemental Shield", 66, 11, 28, 51, 0, 30, 60, 30, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateShield(17, 1, 18, 2, 2, "CrimsonGlory", 104, 19, 90, 51, 0, 95, 48, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateShield(18, 1, 18, 2, 2, "Salamander Shield", 102, 20, 96, 51, 0, 80, 61, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateShield(19, 1, 0, 2, 2, "Frost Barrier", 99, 14, 58, 51, 0, 26, 53, 26, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateShield(20, 1, 18, 2, 2, "Guardian Shiled", 106, 12, 30, 51, 0, 54, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateShield(21, 1, 18, 2, 2, "Cross Shield", 70, 16, 75, 65, 0, 140, 55, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);

        // Helmets:
        this.CreateArmor(0, 2, 2, 2, "Bronze Helm", 16, 9, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0);
        this.CreateArmor(1, 2, 2, 2, "Dragon Helm", 57, 24, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(2, 2, 2, 2, "Pad Helm", 5, 4, 28, 0, 20, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(3, 2, 2, 2, "Legendary Helm", 50, 18, 42, 0, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(4, 2, 2, 2, "Bone Helm", 18, 9, 30, 0, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(5, 2, 2, 2, "Leather Helm", 6, 5, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1);
        this.CreateArmor(6, 2, 2, 2, "Scale Helm", 26, 12, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1);
        this.CreateArmor(7, 2, 2, 2, "Sphinx Mask", 32, 13, 36, 0, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(8, 2, 2, 2, "Brass Helm", 36, 17, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        this.CreateArmor(9, 2, 2, 2, "Plate Helm", 46, 20, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        this.CreateArmor(10, 2, 2, 2, "Vine Helm", 6, 4, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 2, 2, 2, "Silk Helm", 16, 8, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 2, 2, 2, "Wind Helm", 28, 12, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 2, 2, 2, "Spirit Helm", 40, 16, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 2, 2, 2, "Guardian Helm", 53, 23, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(16, 2, 2, 2, "Black Dragon Helm", 82, 30, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 2, 2, 2, "Dark Phoenix Helm", 92, 43, 80, 0, 205, 62, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 2, 2, 2, "Grand Soul Helm", 81, 27, 67, 0, 59, 20, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 2, 2, 2, "Divine Helm", 85, 37, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(21, 2, 2, 2, "Great Dragon Helm", 104, 53, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 2, 2, 2, "Dark Soul Helm", 110, 36, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(24, 2, 2, 2, "Red Spirit Helm", 93, 46, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 2, 2, 2, "Light Plate Mask", 46, 20, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 2, 2, 2, "Adamantine Mask", 66, 24, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 2, 2, 2, "Dark Steel Mask", 86, 26, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 2, 2, 2, "Dark Master Mask", 106, 34, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 2, 2, 2, "Dragon Knight Helm", 130, 66, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 2, 2, 2, "Venom Mist Helm", 126, 48, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 2, 2, 2, "Sylphid Ray Helm", 126, 57, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(33, 2, 2, 2, "Sunlight Mask", 130, 46, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 2, 2, 2, "Ashcrow Helm", 67, 27, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 2, 2, 2, "Eclipse Helm", 67, 22, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 2, 2, 2, "Iris Helm", 67, 30, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(38, 2, 2, 2, "Glorious Mask", 97, 30, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 2, 2, 2, "Mistery Helm", 28, 13, 36, 0, 31, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 2, 2, 2, "Red Wing Helm", 50, 18, 42, 0, 26, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 2, 2, 2, "Ancient Helm", 68, 24, 54, 0, 52, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 2, 2, 2, "Black Rose Helm", 81, 32, 67, 0, 60, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 2, 2, 2, "Aura Helm", 110, 43, 75, 0, 56, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 2, 2, 2, "Lilium Helm", 110, 50, 80, 0, 80, 50, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 2, 2, 2, "Titan Helm", 111, 63, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 2, 2, 2, "Brave Helm", 107, 51, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(49, 2, 2, 2, "Seraphim Helm", 111, 50, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 2, 2, 2, "Faith Helm", 104, 44, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 2, 2, 2, "Paewang Mask", 111, 44, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 2, 2, 2, "Hades Helm", 109, 41, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 2, 2, 2, "Sacred Helm", 54, 24, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 2, 2, 2, "Storm Hard Helm", 70, 32, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 2, 2, 2, "Piercing Helm", 90, 45, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 2, 2, 2, "Phoenix Soul Helmet", 128, 60, 88, 0, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        // Armors:
        this.CreateArmor(0, 3, 2, 2, "Bronze Armor", 18, 14, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(1, 3, 2, 3, "Dragon Armor", 59, 37, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(2, 3, 2, 2, "Pad Armor", 10, 7, 28, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(3, 3, 2, 2, "Legendary Armor", 56, 22, 42, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(4, 3, 2, 2, "Bone Armor", 22, 13, 30, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(5, 3, 2, 3, "Leather Armor", 10, 10, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(6, 3, 2, 2, "Scale Armor", 28, 18, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(7, 3, 2, 3, "Sphinx Armor", 38, 17, 36, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(8, 3, 2, 2, "Brass Armor", 38, 22, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(9, 3, 2, 2, "Plate Armor", 48, 30, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(10, 3, 2, 2, "Vine Armor", 10, 8, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 3, 2, 2, "Silk Armor", 20, 12, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 3, 2, 2, "Wind Armor", 32, 16, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 3, 2, 2, "Spirit Armor", 44, 21, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 3, 2, 2, "Guardian Armor", 57, 29, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(15, 3, 2, 3, "Storm Crow Armor", 80, 44, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(16, 3, 2, 3, "Black Dragon Armor", 90, 48, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 3, 2, 3, "Dark Phoenix Armor", 100, 63, 80, 0, 214, 65, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 3, 2, 3, "Grand Soul Armor", 91, 33, 67, 0, 59, 20, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 3, 2, 2, "Divine Armor", 92, 44, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(20, 3, 2, 3, "Thunder Hawk Armor", 107, 60, 82, 0, 170, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(21, 3, 2, 3, "Great Dragon Armor", 126, 75, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 3, 2, 3, "Dark Soul Armor", 122, 43, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(23, 3, 2, 3, "Hurricane Armor", 128, 73, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(24, 3, 2, 2, "Red Sprit Armor", 109, 55, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 3, 2, 3, "Light Plate Armor", 62, 25, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 3, 2, 3, "Adamantine Armor", 78, 36, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 3, 2, 3, "Dark Steel Armor", 96, 43, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 3, 2, 3, "Dark Master Armor", 117, 51, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 3, 2, 3, "Dragon Knight Armor", 140, 88, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 3, 2, 3, "Venom Mist Armor", 146, 57, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 3, 2, 3, "Sylphid Ray Armor", 146, 68, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(32, 3, 2, 3, "Volcano Armor", 147, 86, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(33, 3, 2, 3, "Sunlight Armor", 147, 64, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 3, 2, 3, "Ashcrow Armor", 75, 42, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 3, 2, 3, "Eclipse Armor", 75, 27, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 3, 2, 3, "Iris Armor", 75, 36, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(37, 3, 2, 3, "Valiant Armor", 105, 52, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(38, 3, 2, 3, "Glorious Armor", 105, 47, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 3, 2, 2, "Mistery Armor", 34, 22, 36, 0, 39, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 3, 2, 2, "Red Wing Armor", 56, 28, 42, 0, 35, 8, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 3, 2, 2, "Ancient Armor", 75, 35, 54, 0, 52, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 3, 2, 2, "Black Rose Armor", 91, 45, 67, 0, 60, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 3, 2, 2, "Aura Armor", 122, 56, 75, 0, 57, 19, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 3, 2, 2, "Lilium Armor", 113, 71, 84, 0, 110, 50, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 3, 2, 3, "Titan Armor", 132, 81, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 3, 2, 3, "Brave Armor", 128, 62, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(47, 3, 2, 3, "Destory Armor", 131, 80, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(48, 3, 2, 3, "Phantom Armor", 125, 66, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(49, 3, 2, 2, "Seraphim Armor", 129, 60, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 3, 2, 2, "Faith Armor", 122, 52, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 3, 2, 3, "Paewang Armor", 132, 58, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 3, 2, 3, "Hades Armor", 129, 50, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 3, 2, 3, "Sacred Armor", 66, 43, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 3, 2, 3, "Storm Hard Armor", 82, 51, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 3, 2, 3, "Piercing Armor", 101, 59, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 3, 2, 3, "Phoenix Soul Armor", 143, 78, 88, 0, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        // Pants:
        this.CreateArmor(0, 4, 2, 2, "Bronze Pants", 15, 10, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(1, 4, 2, 2, "Dragon Pants", 55, 26, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(2, 4, 2, 2, "Pad Pants", 8, 5, 28, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(3, 4, 2, 2, "Legendary Pants", 53, 20, 42, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(4, 4, 2, 2, "Bone Pants", 20, 10, 30, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(5, 4, 2, 2, "Leather Pants", 8, 7, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(6, 4, 2, 2, "Scale Pants", 25, 14, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(7, 4, 2, 2, "Sphinx Pants", 34, 15, 36, 0, 40, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(8, 4, 2, 2, "Brass Pants", 35, 18, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(9, 4, 2, 2, "Plate Pants", 45, 22, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(10, 4, 2, 2, "Vine Pants", 8, 6, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 4, 2, 2, "Silk Pants", 18, 10, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 4, 2, 2, "Wind Pants", 30, 14, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 4, 2, 2, "Spirit Pants", 42, 18, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 4, 2, 2, "Guardian Pants", 54, 25, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(15, 4, 2, 2, "Storm Crow Pants", 74, 34, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(16, 4, 2, 2, "Black Dragon Pants", 84, 40, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 4, 2, 2, "Dark Phoenix Pants", 96, 54, 80, 0, 207, 63, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 4, 2, 2, "Grand Soul Pants", 86, 30, 67, 0, 59, 20, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 4, 2, 2, "Divine Pants", 88, 39, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(20, 4, 2, 2, "Thunder Hawk Pants", 99, 49, 82, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(21, 4, 2, 2, "Great Dragon Pants", 113, 65, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 4, 2, 2, "Dark Soul Pants", 117, 39, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(23, 4, 2, 2, "Hurricane Pants", 122, 61, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(24, 4, 2, 2, "Red Spirit Pants", 100, 48, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 4, 2, 2, "Light Plate Pants", 50, 21, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 4, 2, 2, "Adamantine Pants", 70, 26, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 4, 2, 2, "Dark Steel Pants", 92, 31, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 4, 2, 2, "Dark Master Pants", 110, 39, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 4, 2, 2, "Dragon Knight Pants", 134, 78, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 4, 2, 2, "Venom Mist Pants", 135, 55, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 4, 2, 2, "Sylphid Ray Pants", 135, 61, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(32, 4, 2, 2, "Volcano Pants", 135, 74, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(33, 4, 2, 2, "Sunlight Pants", 140, 52, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 4, 2, 2, "Ashcrow Pants", 69, 33, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 4, 2, 2, "Eclipse Pants", 69, 25, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 4, 2, 2, "Iris Pants", 69, 32, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(37, 4, 2, 2, "Valiant Pants", 101, 41, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(38, 4, 2, 2, "Glorious Pants", 101, 35, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 4, 2, 2, "Mistery Pants", 30, 16, 36, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 4, 2, 2, "Red Wing Pants", 53, 22, 42, 0, 35, 7, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 4, 2, 2, "Ancient Pants", 72, 28, 54, 0, 49, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 4, 2, 2, "Black Rose Pants", 86, 37, 67, 0, 60, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 4, 2, 2, "Aura Pants", 117, 49, 75, 0, 57, 19, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 4, 2, 2, "Lilium Pants", 102, 52, 82, 0, 75, 30, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 4, 2, 2, "Titan Pants", 116, 74, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 4, 2, 2, "Brave Pants", 112, 58, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(47, 4, 2, 2, "Destory Pants", 115, 66, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(48, 4, 2, 2, "Phantom Pants", 113, 51, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(49, 4, 2, 2, "Seraphim Pants", 116, 53, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 4, 2, 2, "Faith Pants", 109, 46, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 4, 2, 2, "Paewang Pants", 116, 46, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 4, 2, 2, "Hades Pants", 114, 44, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 4, 2, 2, "Sacred Pants", 62, 33, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 4, 2, 2, "Storm Hard Pants", 78, 41, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 4, 2, 2, "Piercing Pants", 95, 49, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 4, 2, 2, "Phoenix Soul Pants", 134, 68, 88, 0, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        // Gloves:
        this.CreateArmor(0, 5, 2, 2, "Bronze Gloves", 13, 4, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(1, 5, 2, 2, "Dragon Gloves", 52, 14, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(2, 5, 2, 2, "Pad Gloves", 3, 2, 28, 0, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(3, 5, 2, 2, "Legendary Gloves", 44, 11, 42, 0, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(4, 5, 2, 2, "Bone Gloves", 14, 5, 30, 0, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(5, 5, 2, 2, "Leather Gloves", 4, 2, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(6, 5, 2, 2, "Scale Gloves", 22, 7, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(7, 5, 2, 2, "Sphinx Gloves", 28, 8, 36, 0, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(8, 5, 2, 2, "Brass Gloves", 32, 9, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(9, 5, 2, 2, "Plate Gloves", 42, 12, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(10, 5, 2, 2, "Vine Gloves", 4, 2, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 5, 2, 2, "Silk Gloves", 14, 4, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 5, 2, 2, "Wind Gloves", 26, 6, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 5, 2, 2, "Spirit Gloves", 38, 9, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 5, 2, 2, "Guardian Gloves", 50, 15, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(15, 5, 2, 2, "Storm Crow Gloves", 70, 20, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(16, 5, 2, 2, "Black Dragon Gloves", 76, 22, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 5, 2, 2, "Dark Phoenix Gloves", 86, 37, 80, 0, 205, 63, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 5, 2, 2, "Grand Soul Gloves", 70, 20, 67, 0, 49, 10, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 5, 2, 2, "Divine Gloves", 72, 29, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(20, 5, 2, 2, "Thunder Hawk Gloves", 88, 34, 82, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(21, 5, 2, 2, "Great Dragon Gloves", 94, 48, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 5, 2, 2, "Dark Soul Gloves", 87, 30, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(23, 5, 2, 2, "Hurricane Gloves", 102, 45, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(24, 5, 2, 2, "Red Spirit Gloves", 84, 38, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 5, 2, 2, "Light Plate Gloves", 42, 12, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 5, 2, 2, "Adamantine Gloves", 57, 18, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 5, 2, 2, "Dark Steel Gloves", 75, 21, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 5, 2, 2, "Dark Master Gloves", 89, 29, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 5, 2, 2, "Dragon Knight Gloves", 114, 60, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 5, 2, 2, "Venom Mist Gloves", 111, 44, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 5, 2, 2, "Sylphid Ray Gloves", 111, 50, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(32, 5, 2, 2, "Volcano Gloves", 127, 55, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(33, 5, 2, 2, "Sunlight Gloves", 110, 40, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 5, 2, 2, "Ashcrow Gloves", 61, 18, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 5, 2, 2, "Eclipse Gloves", 61, 15, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 5, 2, 2, "Iris Gloves", 61, 22, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(37, 5, 2, 2, "Valiant Gloves", 91, 27, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(38, 5, 2, 2, "Glorious Gloves", 91, 25, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 5, 2, 2, "Mistery Gloves", 24, 9, 36, 0, 22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 5, 2, 2, "Red Wing Gloves", 44, 13, 42, 0, 18, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 5, 2, 2, "Ancient Gloves", 61, 19, 54, 0, 52, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 5, 2, 2, "Black Rose Gloves", 70, 26, 67, 0, 50, 10, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 5, 2, 2, "Aura Gloves", 87, 34, 75, 0, 56, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 5, 2, 2, "Lilium Gloves", 82, 45, 80, 0, 75, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 5, 2, 2, "Titan Gloves", 100, 56, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 5, 2, 2, "Brave Gloves", 97, 42, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(47, 5, 2, 2, "Destory Gloves", 101, 49, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(48, 5, 2, 2, "Phantom Gloves", 99, 40, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(49, 5, 2, 2, "Seraphim Gloves", 100, 43, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 5, 2, 2, "Faith Gloves", 95, 36, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 5, 2, 2, "Paewang Gloves", 101, 34, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 5, 2, 2, "Hades Gloves", 100, 31, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);

        // Boots:
        this.CreateArmor(0, 6, 2, 2, "Bronze Boots", 12, 4, 34, 0, 80, 20, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateArmor(1, 6, 2, 2, "Dragon Boots", 54, 15, 68, 0, 120, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateArmor(2, 6, 2, 2, "Pad Boots", 4, 3, 28, 0, 20, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(3, 6, 2, 2, "Legendary Boots", 46, 12, 42, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(4, 6, 2, 2, "Bone Boots", 16, 6, 30, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(5, 6, 2, 2, "Leather Boots", 5, 2, 30, 0, 80, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(6, 6, 2, 2, "Scale Boots", 22, 8, 40, 0, 110, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateArmor(7, 6, 2, 2, "Sphinx Boots", 30, 9, 36, 0, 30, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(8, 6, 2, 2, "Brass Boots", 32, 10, 44, 0, 100, 30, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(9, 6, 2, 2, "Plate Boots", 42, 12, 50, 0, 130, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateArmor(10, 6, 2, 2, "Vine Boots", 5, 2, 22, 0, 30, 60, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(11, 6, 2, 2, "Silk Boots", 15, 4, 26, 0, 30, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(12, 6, 2, 2, "Wind Boots", 27, 7, 32, 0, 30, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(13, 6, 2, 2, "Spirit Boots", 40, 10, 38, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(14, 6, 2, 2, "Guardian Boots", 52, 16, 45, 0, 40, 80, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(15, 6, 2, 2, "Storm Crow Boots", 72, 22, 80, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(16, 6, 2, 2, "Black Dragon Boots", 78, 24, 74, 0, 170, 60, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(17, 6, 2, 2, "Dark Phoenix Boots", 93, 40, 80, 0, 198, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(18, 6, 2, 2, "Grand Soul Boots", 76, 22, 67, 0, 59, 10, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(19, 6, 2, 2, "Divine Boots", 81, 30, 74, 0, 50, 110, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(20, 6, 2, 2, "Thunder Hawk Boots", 92, 37, 82, 0, 150, 70, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(21, 6, 2, 2, "Great Dragon Boots", 98, 50, 86, 0, 200, 58, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(22, 6, 2, 2, "Dark Soul Boots", 95, 31, 75, 0, 55, 18, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(23, 6, 2, 2, "Hurricane Boots", 110, 50, 90, 0, 162, 66, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(24, 6, 2, 2, "Red Spirit Boots", 87, 40, 80, 0, 52, 115, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(25, 6, 2, 2, "Light Plate Boots", 45, 13, 42, 0, 70, 20, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(26, 6, 2, 2, "Adamantine Boots", 60, 20, 56, 0, 77, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(27, 6, 2, 2, "Dark Steel Boots", 83, 25, 70, 0, 84, 22, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(28, 6, 2, 2, "Dark Master Boots", 95, 33, 78, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(29, 6, 2, 2, "Dragon Knight Boots", 119, 63, 90, 380, 170, 60, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateArmor(30, 6, 2, 2, "Venom Mist Boots", 119, 47, 86, 380, 44, 15, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(31, 6, 2, 2, "Sylphid Ray Boots", 119, 53, 86, 380, 38, 80, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateArmor(32, 6, 2, 2, "Volcano Boots", 131, 61, 95, 380, 145, 60, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(33, 6, 2, 2, "Sunlight Boots", 121, 44, 82, 380, 62, 16, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(34, 6, 2, 2, "Ashcrow Boots", 68, 19, 72, 0, 160, 50, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(35, 6, 2, 2, "Eclipse Boots", 68, 17, 54, 0, 53, 12, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(36, 6, 2, 2, "Iris Boots", 68, 23, 59, 0, 50, 70, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(37, 6, 2, 2, "Valiant Boots", 98, 29, 81, 0, 155, 50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(38, 6, 2, 2, "Glorious Boots", 98, 29, 74, 0, 80, 21, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(39, 6, 2, 2, "Mistery Boots", 26, 11, 36, 0, 27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(40, 6, 2, 2, "Red Wing Boots", 46, 15, 42, 0, 25, 4, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(41, 6, 2, 2, "Ancient Boots", 65, 21, 54, 0, 53, 16, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateArmor(42, 6, 2, 2, "Black Rose Boots", 76, 28, 67, 0, 60, 10, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(43, 6, 2, 2, "Aura Boots", 95, 38, 75, 0, 57, 20, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(44, 6, 2, 2, "Lilium Boots", 90, 50, 85, 0, 150, 30, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateArmor(45, 6, 2, 2, "Titan Boots", 96, 57, 86, 0, 222, 32, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(46, 6, 2, 2, "Brave Boots", 93, 45, 86, 0, 74, 162, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        this.CreateArmor(47, 6, 2, 2, "Destory Boots", 97, 54, 86, 0, 212, 57, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(48, 6, 2, 2, "Phantom Boots", 94, 44, 86, 0, 62, 19, 165, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateArmor(49, 6, 2, 2, "Seraphim Boots", 97, 42, 86, 0, 55, 197, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(50, 6, 2, 2, "Faith Boots", 92, 35, 86, 0, 32, 29, 138, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateArmor(51, 6, 2, 2, "Phaewang Boots", 98, 38, 86, 0, 105, 38, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateArmor(52, 6, 2, 2, "Hades Boots", 94, 34, 86, 0, 60, 15, 181, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        this.CreateArmor(59, 6, 2, 2, "Sacred Boots", 50, 20, 52, 1, 85, 0, 0, 75, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(60, 6, 2, 2, "Storm Hard Boots", 62, 28, 68, 1, 100, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(61, 6, 2, 2, "Piercing Boots", 82, 36, 82, 1, 115, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateArmor(73, 6, 2, 2, "Phoenix Soul Boots", 119, 57, 88, 0, 97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

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