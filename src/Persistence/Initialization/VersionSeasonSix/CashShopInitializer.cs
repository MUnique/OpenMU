// <copyright file="CashShopInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Initializes the cash shop configuration.
/// </summary>
/// <remarks>
/// The packages and products match the cash shop script 512.2012.084, which comes with the
/// game client in 'Data\InGameShopScript\512.2012.084'. The client shows the packages of this
/// script and sends their sequence numbers when a package is bought, so this data has to
/// match the script of the configured version.
/// The script doesn't define the item level, so it's taken from the product names,
/// e.g. 'Box of Kundun +3' or 'Bundle of Jewel of Soul (30)'.
/// Products whose item isn't defined in this configuration can't be delivered, and
/// time-limited products neither, because items which expire aren't supported yet.
/// Their packages are therefore initialized as not for sale.
/// </remarks>
internal sealed class CashShopInitializer : InitializerBase
{
    // The script and banner versions which come with the game client.
    private const short ScriptSaleZone = 512;

    private const short ScriptYear = 2012;

    private const short ScriptYearId = 84;

    private const short BannerSaleZone = 583;

    private const short BannerYear = 2011;

    private const short BannerYearId = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashShopInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CashShopInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        if (this.GameConfiguration.CashShopConfiguration is not null)
        {
            return;
        }

        var configuration = this.Context.CreateNew<CashShopConfiguration>();
        configuration.ScriptSaleZone = ScriptSaleZone;
        configuration.ScriptYear = ScriptYear;
        configuration.ScriptYearId = ScriptYearId;
        configuration.BannerSaleZone = BannerSaleZone;
        configuration.BannerYear = BannerYear;
        configuration.BannerYearId = BannerYearId;
        this.GameConfiguration.CashShopConfiguration = configuration;

        this.AddPackages(configuration);
    }

    private void AddPackages(CashShopConfiguration configuration)
    {
        this.AddPackage(configuration, 413, LocalizedString.FromResource(() => CashShopPackageNames.GoldChannelTicket), 190, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(528, 607, 190, 13, 124, 0, 1, 86400)
            .AddProduct(528, 608, 400, 13, 124, 0, 1, 259200)
            .AddProduct(528, 609, 800, 13, 124, 0, 1, 604800)
            .AddProduct(528, 610, 2300, 13, 124, 0, 1, 2592000);
        this.AddPackage(configuration, 260, LocalizedString.FromResource(() => CashShopPackageNames.MagicBackpack), 690, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(281, 349, 690, 14, 162, 0, 1, 0);
        this.AddPackage(configuration, 258, LocalizedString.FromResource(() => CashShopPackageNames.VaultExpansionCertificate), 2490, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(282, 350, 2490, 14, 163, 0, 1, 0);
        this.AddPackage(configuration, 187, LocalizedString.FromResource(() => CashShopPackageNames.PremiumService), 0, CashShopCoinType.WCoinC, isForSale: false, isGiftable: false, isBundle: false)
            .AddProduct(149, 209, 0, 14, 137, 0, 1, 0);
        this.AddPackage(configuration, 162, LocalizedString.FromResource(() => CashShopPackageNames.GladiatorPackage), 500, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: true)
            .AddProduct(115, 174, 0, 14, 72, 0, 1, 86400)
            .AddProduct(116, 175, 0, 14, 133, 0, 10, 0)
            .AddProduct(118, 177, 130, 13, 107, 0, 1, 86400)
            .AddProduct(119, 178, 100, 13, 64, 0, 1, 86400)
            .AddProduct(120, 179, 150, 14, 98, 0, 1, 86400)
            .AddProduct(121, 180, 120, 13, 105, 0, 1, 86400)
            .AddProduct(122, 181, 0, 13, 113, 0, 1, 86400);
        this.AddPackage(configuration, 98, LocalizedString.FromResource(() => CashShopPackageNames.TalismanofLuck), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(22, 37, 100, 14, 53, 0, 1, 0)
            .AddProduct(22, 38, 400, 14, 53, 0, 1, 0);
        this.AddPackage(configuration, 114, LocalizedString.FromResource(() => CashShopPackageNames.ResetFruitStrength), 250, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(14, 24, 250, 13, 54, 0, 1, 0);
        this.AddPackage(configuration, 112, LocalizedString.FromResource(() => CashShopPackageNames.ResetFruitQuickness), 250, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(15, 25, 250, 13, 55, 0, 1, 0);
        this.AddPackage(configuration, 110, LocalizedString.FromResource(() => CashShopPackageNames.ResetFruitHealth), 250, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(16, 26, 250, 13, 56, 0, 1, 0);
        this.AddPackage(configuration, 108, LocalizedString.FromResource(() => CashShopPackageNames.ResetFruitEnergy), 250, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(17, 27, 250, 13, 57, 0, 1, 0);
        this.AddPackage(configuration, 106, LocalizedString.FromResource(() => CashShopPackageNames.ResetFruitControl), 250, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(18, 28, 250, 13, 58, 0, 1, 0);
        this.AddPackage(configuration, 263, LocalizedString.FromResource(() => CashShopPackageNames.RageFighterCharacterCard), 500, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(284, 353, 500, 14, 169, 0, 1, 0);
        this.AddPackage(configuration, 126, LocalizedString.FromResource(() => CashShopPackageNames.SummonerCharacterCard), 500, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(31, 53, 500, 14, 91, 0, 1, 0);
        this.AddPackage(configuration, 70, LocalizedString.FromResource(() => CashShopPackageNames.TalismanofResurrection), 50, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(40, 66, 50, 13, 69, 0, 1, 0);
        this.AddPackage(configuration, 68, LocalizedString.FromResource(() => CashShopPackageNames.TalismanofMobility), 40, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(41, 67, 40, 13, 70, 0, 1, 0);
        this.AddPackage(configuration, 122, LocalizedString.FromResource(() => CashShopPackageNames.SealofWealth), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(10, 13, 150, 13, 44, 0, 1, 86400)
            .AddProduct(10, 14, 350, 13, 44, 0, 1, 259200)
            .AddProduct(10, 15, 700, 13, 44, 0, 1, 604800)
            .AddProduct(10, 16, 2100, 13, 44, 0, 1, 2592000);
        this.AddPackage(configuration, 124, LocalizedString.FromResource(() => CashShopPackageNames.SealofAscension), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(9, 9, 100, 13, 43, 0, 1, 86400)
            .AddProduct(9, 10, 250, 13, 43, 0, 1, 259200)
            .AddProduct(9, 11, 500, 13, 43, 0, 1, 604800)
            .AddProduct(9, 12, 1400, 13, 43, 0, 1, 2592000);
        this.AddPackage(configuration, 48, LocalizedString.FromResource(() => CashShopPackageNames.MasterSealofWealth), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(51, 80, 150, 13, 94, 0, 1, 86400)
            .AddProduct(51, 81, 700, 13, 94, 0, 1, 604800)
            .AddProduct(51, 82, 2100, 13, 94, 0, 1, 2592000);
        this.AddPackage(configuration, 50, LocalizedString.FromResource(() => CashShopPackageNames.MasterSealofAscension), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(50, 77, 100, 13, 93, 0, 1, 86400)
            .AddProduct(50, 78, 500, 13, 93, 0, 1, 604800)
            .AddProduct(50, 79, 1400, 13, 93, 0, 1, 2592000);
        this.AddPackage(configuration, 120, LocalizedString.FromResource(() => CashShopPackageNames.SealofSustenance), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(11, 17, 80, 13, 45, 0, 1, 86400)
            .AddProduct(11, 18, 680, 13, 45, 0, 1, 864000)
            .AddProduct(11, 19, 1250, 13, 45, 0, 1, 2592000);
        this.AddPackage(configuration, 74, LocalizedString.FromResource(() => CashShopPackageNames.SealofHealing), 120, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(38, 62, 120, 13, 62, 0, 1, 86400)
            .AddProduct(38, 63, 600, 13, 62, 0, 1, 604800);
        this.AddPackage(configuration, 72, LocalizedString.FromResource(() => CashShopPackageNames.SealofDivinity), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(39, 64, 100, 13, 63, 0, 1, 86400)
            .AddProduct(39, 65, 500, 13, 63, 0, 1, 604800);
        this.AddPackage(configuration, 9, LocalizedString.FromResource(() => CashShopPackageNames.AGBoostAura), 120, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(73, 123, 120, 13, 104, 0, 1, 86400)
            .AddProduct(73, 124, 600, 13, 104, 0, 1, 604800);
        this.AddPackage(configuration, 6, LocalizedString.FromResource(() => CashShopPackageNames.SDBoostAura), 120, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(74, 125, 120, 13, 105, 0, 1, 86400)
            .AddProduct(74, 126, 600, 13, 105, 0, 1, 604800);
        this.AddPackage(configuration, 356, LocalizedString.FromResource(() => CashShopPackageNames.WornHorseshoe), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(463, 540, 80, 13, 134, 0, 1, 86400)
            .AddProduct(463, 541, 400, 13, 134, 0, 1, 604800)
            .AddProduct(463, 542, 1200, 13, 134, 0, 1, 2592000);
        this.AddPackage(configuration, 354, LocalizedString.FromResource(() => CashShopPackageNames.GoldenOakCharm), 120, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(462, 537, 120, 13, 132, 0, 1, 86400)
            .AddProduct(462, 538, 600, 13, 132, 0, 1, 604800)
            .AddProduct(462, 539, 1800, 13, 132, 0, 1, 2592000);
        this.AddPackage(configuration, 352, LocalizedString.FromResource(() => CashShopPackageNames.OakCharm), 30, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(461, 534, 30, 13, 130, 0, 1, 86400)
            .AddProduct(461, 535, 150, 13, 130, 0, 1, 604800)
            .AddProduct(461, 536, 500, 13, 130, 0, 1, 2592000);
        this.AddPackage(configuration, 348, LocalizedString.FromResource(() => CashShopPackageNames.StatueofHawk), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(459, 528, 80, 13, 128, 0, 1, 86400)
            .AddProduct(459, 529, 400, 13, 128, 0, 1, 604800)
            .AddProduct(459, 530, 1200, 13, 128, 0, 1, 2592000);
        this.AddPackage(configuration, 350, LocalizedString.FromResource(() => CashShopPackageNames.StatueofGoral), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(460, 531, 80, 13, 129, 0, 1, 86400)
            .AddProduct(460, 532, 400, 13, 129, 0, 1, 604800)
            .AddProduct(460, 533, 1200, 13, 129, 0, 1, 2592000);
        this.AddPackage(configuration, 197, LocalizedString.FromResource(() => CashShopPackageNames.ScrollPackageA), 320, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: true)
            .AddProduct(159, 219, 120, 14, 74, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400);
        this.AddPackage(configuration, 199, LocalizedString.FromResource(() => CashShopPackageNames.ScrollPackageB), 320, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: true)
            .AddProduct(158, 218, 120, 14, 75, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400);
        this.AddPackage(configuration, 201, LocalizedString.FromResource(() => CashShopPackageNames.ScrollPackageC), 488, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: true)
            .AddProduct(156, 216, 56, 14, 77, 0, 1, 86400)
            .AddProduct(157, 217, 56, 14, 76, 0, 1, 86400)
            .AddProduct(159, 219, 120, 14, 74, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400)
            .AddProduct(162, 222, 56, 14, 97, 0, 1, 86400);
        this.AddPackage(configuration, 203, LocalizedString.FromResource(() => CashShopPackageNames.ScrollPackageD), 488, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: true)
            .AddProduct(156, 216, 56, 14, 77, 0, 1, 86400)
            .AddProduct(157, 217, 56, 14, 76, 0, 1, 86400)
            .AddProduct(158, 218, 120, 14, 75, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400)
            .AddProduct(162, 222, 56, 14, 97, 0, 1, 86400);
        this.AddPackage(configuration, 90, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofProtection), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(26, 43, 100, 14, 73, 0, 1, 86400)
            .AddProduct(26, 44, 500, 14, 73, 0, 1, 604800);
        this.AddPackage(configuration, 92, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofQuickness), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(25, 41, 150, 14, 72, 0, 1, 86400)
            .AddProduct(25, 42, 700, 14, 72, 0, 1, 604800);
        this.AddPackage(configuration, 88, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofWrath), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(27, 45, 150, 14, 74, 0, 1, 86400)
            .AddProduct(27, 46, 700, 14, 74, 0, 1, 604800);
        this.AddPackage(configuration, 86, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofWizardry), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(28, 47, 150, 14, 75, 0, 1, 86400)
            .AddProduct(28, 48, 700, 14, 75, 0, 1, 604800);
        this.AddPackage(configuration, 76, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofStrengthener), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(37, 60, 150, 14, 98, 0, 1, 86400)
            .AddProduct(37, 61, 700, 14, 98, 0, 1, 604800);
        this.AddPackage(configuration, 78, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofBattle), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(36, 58, 150, 14, 97, 0, 1, 86400)
            .AddProduct(36, 59, 700, 14, 97, 0, 1, 604800);
        this.AddPackage(configuration, 82, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofMana), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(30, 51, 100, 14, 77, 0, 1, 86400)
            .AddProduct(30, 52, 500, 14, 77, 0, 1, 604800);
        this.AddPackage(configuration, 84, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofHealth), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(29, 49, 100, 14, 76, 0, 1, 86400)
            .AddProduct(29, 50, 500, 14, 76, 0, 1, 604800);
        this.AddPackage(configuration, 30, LocalizedString.FromResource(() => CashShopPackageNames.PetPanda), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(60, 99, 150, 13, 80, 0, 1, 86400)
            .AddProduct(60, 100, 700, 13, 80, 0, 1, 604800);
        this.AddPackage(configuration, 28, LocalizedString.FromResource(() => CashShopPackageNames.PetSkeleton), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(61, 101, 150, 13, 123, 0, 1, 86400)
            .AddProduct(61, 102, 700, 13, 123, 0, 1, 604800);
        this.AddPackage(configuration, 10, LocalizedString.FromResource(() => CashShopPackageNames.PetUnicorn), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(72, 121, 150, 13, 106, 0, 1, 86400)
            .AddProduct(72, 122, 700, 13, 106, 0, 1, 604800);
        this.AddPackage(configuration, 102, LocalizedString.FromResource(() => CashShopPackageNames.Demon), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(20, 31, 100, 13, 64, 0, 1, 86400)
            .AddProduct(20, 32, 500, 13, 64, 0, 1, 604800)
            .AddProduct(20, 33, 1400, 13, 64, 0, 1, 2592000);
        this.AddPackage(configuration, 100, LocalizedString.FromResource(() => CashShopPackageNames.SpiritofGuardian), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(21, 34, 100, 13, 65, 0, 1, 86400)
            .AddProduct(21, 35, 500, 13, 65, 0, 1, 604800)
            .AddProduct(21, 36, 1400, 13, 65, 0, 1, 2592000);
        this.AddPackage(configuration, 36, LocalizedString.FromResource(() => CashShopPackageNames.PandaRing), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(57, 93, 100, 13, 76, 0, 1, 86400)
            .AddProduct(57, 94, 500, 13, 76, 0, 1, 604800);
        this.AddPackage(configuration, 32, LocalizedString.FromResource(() => CashShopPackageNames.SkeletonTransformationRing), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(59, 97, 100, 13, 122, 0, 1, 86400)
            .AddProduct(59, 98, 500, 13, 122, 0, 1, 604800);
        this.AddPackage(configuration, 34, LocalizedString.FromResource(() => CashShopPackageNames.WizardRing), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(58, 95, 100, 13, 20, 0, 1, 86400)
            .AddProduct(58, 96, 500, 13, 20, 0, 1, 604800);
        this.AddPackage(configuration, 147, LocalizedString.FromResource(() => CashShopPackageNames.CriticalRingofWizard), 130, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(71, 119, 130, 13, 107, 0, 1, 86400)
            .AddProduct(71, 120, 600, 13, 107, 0, 1, 604800);
        this.AddPackage(configuration, 26, LocalizedString.FromResource(() => CashShopPackageNames.SapphireRing), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(62, 103, 80, 13, 109, 0, 1, 86400)
            .AddProduct(62, 104, 400, 13, 109, 0, 1, 604800);
        this.AddPackage(configuration, 24, LocalizedString.FromResource(() => CashShopPackageNames.RubyRing), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(63, 105, 80, 13, 110, 0, 1, 86400)
            .AddProduct(63, 106, 400, 13, 110, 0, 1, 604800);
        this.AddPackage(configuration, 22, LocalizedString.FromResource(() => CashShopPackageNames.TopazRing), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(64, 107, 80, 13, 111, 0, 1, 86400)
            .AddProduct(64, 108, 400, 13, 111, 0, 1, 604800);
        this.AddPackage(configuration, 20, LocalizedString.FromResource(() => CashShopPackageNames.AmethystRing), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(65, 109, 80, 13, 112, 0, 1, 86400)
            .AddProduct(65, 110, 400, 13, 112, 0, 1, 604800);
        this.AddPackage(configuration, 18, LocalizedString.FromResource(() => CashShopPackageNames.RubyNecklace), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(66, 111, 90, 13, 113, 0, 1, 86400)
            .AddProduct(66, 112, 450, 13, 113, 0, 1, 604800);
        this.AddPackage(configuration, 16, LocalizedString.FromResource(() => CashShopPackageNames.EmeraldNecklace), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(67, 113, 90, 13, 114, 0, 1, 86400)
            .AddProduct(67, 114, 450, 13, 114, 0, 1, 604800);
        this.AddPackage(configuration, 14, LocalizedString.FromResource(() => CashShopPackageNames.SapphireNecklace), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(68, 115, 90, 13, 115, 0, 1, 86400)
            .AddProduct(68, 116, 450, 13, 115, 0, 1, 604800);
        this.AddPackage(configuration, 256, LocalizedString.FromResource(() => CashShopPackageNames.LittleWarriorsCloak), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(283, 351, 90, 12, 135, 0, 1, 86400)
            .AddProduct(283, 352, 450, 12, 135, 0, 1, 604800);
        this.AddPackage(configuration, 44, LocalizedString.FromResource(() => CashShopPackageNames.SmallWingsofDisaster), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(53, 85, 90, 12, 131, 0, 1, 86400)
            .AddProduct(53, 86, 450, 12, 131, 0, 1, 604800);
        this.AddPackage(configuration, 42, LocalizedString.FromResource(() => CashShopPackageNames.SmallWingsofFairy), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(54, 87, 90, 12, 132, 0, 1, 86400)
            .AddProduct(54, 88, 450, 12, 132, 0, 1, 604800);
        this.AddPackage(configuration, 40, LocalizedString.FromResource(() => CashShopPackageNames.SmallWingsofHeaven), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(55, 89, 90, 12, 133, 0, 1, 86400)
            .AddProduct(55, 90, 450, 12, 133, 0, 1, 604800);
        this.AddPackage(configuration, 38, LocalizedString.FromResource(() => CashShopPackageNames.SmallWingsofSatan), 90, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(56, 91, 90, 12, 134, 0, 1, 86400)
            .AddProduct(56, 92, 450, 12, 134, 0, 1, 604800);
        this.AddPackage(configuration, 118, LocalizedString.FromResource(() => CashShopPackageNames.DevilSquareTicket), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(12, 20, 100, 13, 46, 0, 1, 0)
            .AddProduct(12, 21, 900, 13, 46, 0, 10, 0);
        this.AddPackage(configuration, 116, LocalizedString.FromResource(() => CashShopPackageNames.BloodCastleTicket), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(13, 22, 100, 13, 47, 0, 1, 0)
            .AddProduct(13, 23, 900, 13, 47, 0, 10, 0);
        this.AddPackage(configuration, 104, LocalizedString.FromResource(() => CashShopPackageNames.IllusionTempleTicket), 150, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(19, 29, 150, 13, 61, 0, 1, 0)
            .AddProduct(19, 30, 1350, 13, 61, 0, 10, 0);
        this.AddPackage(configuration, 56, LocalizedString.FromResource(() => CashShopPackageNames.KalimaTicket), 80, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(47, 73, 80, 13, 48, 0, 1, 0)
            .AddProduct(47, 74, 720, 13, 48, 0, 10, 0);
        this.AddPackage(configuration, 3, LocalizedString.FromResource(() => CashShopPackageNames.OpenAccessTickettoVarka7), 100, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(79, 134, 100, 13, 127, 0, 1, 0)
            .AddProduct(79, 135, 900, 13, 127, 0, 10, 0);
        this.AddPackage(configuration, 4, LocalizedString.FromResource(() => CashShopPackageNames.EliteSDPotion), 40, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(76, 129, 40, 14, 133, 0, 10, 0);
        this.AddPackage(configuration, 96, LocalizedString.FromResource(() => CashShopPackageNames.EliteHealingPotion), 20, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(23, 39, 20, 14, 70, 0, 20, 0);
        this.AddPackage(configuration, 94, LocalizedString.FromResource(() => CashShopPackageNames.EliteManaPotion), 20, CashShopCoinType.WCoinC, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(24, 40, 20, 14, 71, 0, 20, 0);
        this.AddPackage(configuration, 2224, LocalizedString.FromResource(() => CashShopPackageNames.NormaleX700OBTCelebrationPackage), 4500, CashShopCoinType.WCoinC, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(1352, 1556, 1125, 13, 124, 0, 1, 2592000)
            .AddProduct(1353, 1557, 1125, 13, 44, 0, 1, 2592000)
            .AddProduct(1354, 1558, 1125, 13, 62, 0, 1, 2592000)
            .AddProduct(1355, 1559, 1125, 13, 80, 0, 1, 2592000);
        this.AddPackage(configuration, 2223, LocalizedString.FromResource(() => CashShopPackageNames.MastereX700OBTCelebrationPackage), 4500, CashShopCoinType.WCoinC, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(1352, 1556, 1125, 13, 124, 0, 1, 2592000)
            .AddProduct(1354, 1558, 1125, 13, 62, 0, 1, 2592000)
            .AddProduct(1355, 1559, 1125, 13, 80, 0, 1, 2592000)
            .AddProduct(1356, 1560, 1125, 13, 94, 0, 1, 2592000);
        this.AddPackage(configuration, 2222, LocalizedString.FromResource(() => CashShopPackageNames.MastereX700OBTCelebrationPackage), 4500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(1352, 1556, 1125, 13, 124, 0, 1, 2592000)
            .AddProduct(1354, 1558, 1125, 13, 62, 0, 1, 2592000)
            .AddProduct(1355, 1559, 1125, 13, 80, 0, 1, 2592000)
            .AddProduct(1356, 1560, 1125, 13, 94, 0, 1, 2592000);
        this.AddPackage(configuration, 2221, LocalizedString.FromResource(() => CashShopPackageNames.NormaleX700OBTCelebrationPackage), 4500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(1352, 1556, 1125, 13, 124, 0, 1, 2592000)
            .AddProduct(1353, 1557, 1125, 13, 44, 0, 1, 2592000)
            .AddProduct(1354, 1558, 1125, 13, 62, 0, 1, 2592000)
            .AddProduct(1355, 1559, 1125, 13, 80, 0, 1, 2592000);
        this.AddPackage(configuration, 141, LocalizedString.FromResource(() => CashShopPackageNames.BracketedChaosCardGold), 350, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(32, 54, 350, 14, 92, 0, 1, 0);
        this.AddPackage(configuration, 81, LocalizedString.FromResource(() => CashShopPackageNames.BracketedChaosCardMini), 120, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(34, 56, 120, 14, 95, 0, 1, 0);
        this.AddPackage(configuration, 2118, LocalizedString.FromResource(() => CashShopPackageNames.TenPlusThreeChaosCardMini), 1200, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(546, 633, 0, 14, 95, 0, 1, 0)
            .AddProduct(547, 634, 0, 14, 95, 0, 1, 0)
            .AddProduct(548, 635, 0, 14, 95, 0, 1, 0)
            .AddProduct(549, 636, 120, 14, 95, 0, 1, 0)
            .AddProduct(550, 637, 120, 14, 95, 0, 1, 0)
            .AddProduct(551, 638, 120, 14, 95, 0, 1, 0)
            .AddProduct(552, 639, 120, 14, 95, 0, 1, 0)
            .AddProduct(553, 640, 120, 14, 95, 0, 1, 0)
            .AddProduct(554, 641, 120, 14, 95, 0, 1, 0)
            .AddProduct(555, 642, 120, 14, 95, 0, 1, 0)
            .AddProduct(556, 643, 120, 14, 95, 0, 1, 0)
            .AddProduct(557, 644, 120, 14, 95, 0, 1, 0)
            .AddProduct(558, 645, 0, 14, 95, 0, 1, 0);
        this.AddPackage(configuration, 2117, LocalizedString.FromResource(() => CashShopPackageNames.TenPlusThreeChaosCardGold), 3500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(530, 612, 0, 14, 92, 0, 1, 0)
            .AddProduct(534, 621, 0, 14, 92, 0, 1, 0)
            .AddProduct(535, 622, 0, 14, 92, 0, 1, 0)
            .AddProduct(536, 623, 350, 14, 92, 0, 1, 0)
            .AddProduct(537, 624, 350, 14, 92, 0, 1, 0)
            .AddProduct(538, 625, 350, 14, 92, 0, 1, 0)
            .AddProduct(539, 626, 350, 14, 92, 0, 1, 0)
            .AddProduct(540, 627, 350, 14, 92, 0, 1, 0)
            .AddProduct(541, 628, 350, 14, 92, 0, 1, 0)
            .AddProduct(542, 629, 350, 14, 92, 0, 1, 0)
            .AddProduct(543, 630, 350, 14, 92, 0, 1, 0)
            .AddProduct(544, 631, 350, 14, 92, 0, 1, 0)
            .AddProduct(545, 632, 0, 14, 92, 0, 1, 0);
        this.AddPackage(configuration, 2116, LocalizedString.FromResource(() => CashShopPackageNames.FivePlusOneChaosCardGold), 1750, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(535, 622, 0, 14, 92, 0, 1, 0)
            .AddProduct(541, 628, 350, 14, 92, 0, 1, 0)
            .AddProduct(542, 629, 350, 14, 92, 0, 1, 0)
            .AddProduct(543, 630, 350, 14, 92, 0, 1, 0)
            .AddProduct(544, 631, 350, 14, 92, 0, 1, 0)
            .AddProduct(545, 632, 0, 14, 92, 0, 1, 0);
        this.AddPackage(configuration, 2115, LocalizedString.FromResource(() => CashShopPackageNames.FivePlusOneChaosCardMini), 600, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(548, 635, 0, 14, 95, 0, 1, 0)
            .AddProduct(554, 641, 120, 14, 95, 0, 1, 0)
            .AddProduct(555, 642, 120, 14, 95, 0, 1, 0)
            .AddProduct(556, 643, 120, 14, 95, 0, 1, 0)
            .AddProduct(557, 644, 120, 14, 95, 0, 1, 0)
            .AddProduct(558, 645, 0, 14, 95, 0, 1, 0);
        this.AddPackage(configuration, 412, LocalizedString.FromResource(() => CashShopPackageNames.BracketedGoldChannelTicket), 190, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(528, 607, 190, 13, 124, 0, 1, 86400)
            .AddProduct(528, 608, 400, 13, 124, 0, 1, 259200)
            .AddProduct(528, 609, 800, 13, 124, 0, 1, 604800)
            .AddProduct(528, 610, 2300, 13, 124, 0, 1, 2592000);
        this.AddPackage(configuration, 424, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPGoldChannelTicket7Day), 800, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(512, 591, 0, 14, 120, 0, 1, 0)
            .AddProduct(526, 605, 800, 13, 124, 0, 1, 604800);
        this.AddPackage(configuration, 422, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPGoldChannelTicket30Day), 2300, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(517, 596, 0, 14, 120, 0, 1, 0)
            .AddProduct(527, 606, 2300, 13, 124, 0, 1, 2592000);
        this.AddPackage(configuration, 344, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkillbookpackageforMagicGladiator), 1500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(431, 500, 200, 13, 80, 0, 1, 604800)
            .AddProduct(432, 501, 600, 13, 62, 0, 1, 604800)
            .AddProduct(433, 502, 200, 12, 30, 0, 1, 0)
            .AddProduct(434, 503, 200, 12, 31, 0, 1, 0)
            .AddProduct(439, 508, 200, 15, 8, 0, 1, 0)
            .AddProduct(455, 524, 200, 15, 29, 0, 1, 0)
            .AddProduct(456, 525, 150, 15, 12, 0, 1, 0)
            .AddProduct(457, 526, 150, 12, 47, 0, 1, 0);
        this.AddPackage(configuration, 343, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkillbookpackageforFairyElf), 1500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(431, 500, 200, 13, 80, 0, 1, 604800)
            .AddProduct(432, 501, 600, 13, 62, 0, 1, 604800)
            .AddProduct(433, 502, 200, 12, 30, 0, 1, 0)
            .AddProduct(434, 503, 200, 12, 31, 0, 1, 0)
            .AddProduct(451, 520, 200, 12, 45, 0, 1, 0)
            .AddProduct(452, 521, 200, 12, 18, 0, 1, 0)
            .AddProduct(453, 522, 150, 12, 17, 0, 1, 0)
            .AddProduct(454, 523, 150, 12, 46, 0, 1, 0);
        this.AddPackage(configuration, 342, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkillbookpackageforRageFighter), 1500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(431, 500, 200, 13, 80, 0, 1, 604800)
            .AddProduct(432, 501, 600, 13, 62, 0, 1, 604800)
            .AddProduct(433, 502, 200, 12, 30, 0, 1, 0)
            .AddProduct(434, 503, 200, 12, 31, 0, 1, 0)
            .AddProduct(447, 516, 200, 15, 35, 0, 1, 0)
            .AddProduct(448, 517, 200, 15, 31, 0, 1, 0)
            .AddProduct(449, 518, 150, 15, 33, 0, 1, 0)
            .AddProduct(450, 519, 150, 15, 36, 0, 1, 0);
        this.AddPackage(configuration, 341, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkillbookpackageforSummoner), 1500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(431, 500, 200, 13, 80, 0, 1, 604800)
            .AddProduct(432, 501, 600, 13, 62, 0, 1, 604800)
            .AddProduct(433, 502, 200, 12, 30, 0, 1, 0)
            .AddProduct(434, 503, 200, 12, 31, 0, 1, 0)
            .AddProduct(443, 512, 200, 15, 19, 0, 1, 0)
            .AddProduct(444, 513, 200, 15, 22, 0, 1, 0)
            .AddProduct(445, 514, 150, 15, 23, 0, 1, 0)
            .AddProduct(446, 515, 150, 15, 21, 0, 1, 0);
        this.AddPackage(configuration, 340, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkillbookpackageforDarkWizard), 1500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(431, 500, 200, 13, 80, 0, 1, 604800)
            .AddProduct(432, 501, 600, 13, 62, 0, 1, 604800)
            .AddProduct(433, 502, 200, 12, 30, 0, 1, 0)
            .AddProduct(434, 503, 200, 12, 31, 0, 1, 0)
            .AddProduct(439, 508, 200, 15, 8, 0, 1, 0)
            .AddProduct(440, 509, 200, 15, 28, 0, 1, 0)
            .AddProduct(441, 510, 150, 15, 18, 0, 1, 0)
            .AddProduct(442, 511, 150, 15, 15, 0, 1, 0);
        this.AddPackage(configuration, 338, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkillbookpackageforDarkKnight), 1500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(431, 500, 200, 13, 80, 0, 1, 604800)
            .AddProduct(432, 501, 600, 13, 62, 0, 1, 604800)
            .AddProduct(433, 502, 200, 12, 30, 0, 1, 0)
            .AddProduct(434, 503, 200, 12, 31, 0, 1, 0)
            .AddProduct(435, 504, 200, 12, 44, 0, 1, 0)
            .AddProduct(436, 505, 200, 12, 12, 0, 1, 0)
            .AddProduct(437, 506, 150, 12, 19, 0, 1, 0)
            .AddProduct(438, 507, 150, 12, 14, 0, 1, 0);
        this.AddPackage(configuration, 261, LocalizedString.FromResource(() => CashShopPackageNames.BracketedMagicBackpack), 690, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(281, 349, 690, 14, 162, 0, 1, 0);
        this.AddPackage(configuration, 259, LocalizedString.FromResource(() => CashShopPackageNames.BracketedVaultExpansionCertificate), 2490, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(282, 350, 2490, 14, 163, 0, 1, 0);
        this.AddPackage(configuration, 146, LocalizedString.FromResource(() => CashShopPackageNames.BracketedLevelupMaster), 1570, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(95, 153, 700, 13, 94, 0, 1, 604800)
            .AddProduct(96, 154, 80, 14, 92, 0, 1, 0)
            .AddProduct(97, 155, 30, 14, 73, 0, 1, 86400)
            .AddProduct(98, 156, 600, 13, 124, 0, 1, 604800)
            .AddProduct(99, 157, 80, 14, 92, 0, 1, 0)
            .AddProduct(100, 158, 80, 14, 92, 0, 1, 0);
        this.AddPackage(configuration, 144, LocalizedString.FromResource(() => CashShopPackageNames.BracketedLevelupnormal), 1570, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(94, 152, 700, 13, 44, 0, 1, 604800)
            .AddProduct(96, 154, 80, 14, 92, 0, 1, 0)
            .AddProduct(97, 155, 30, 14, 73, 0, 1, 86400)
            .AddProduct(98, 156, 600, 13, 124, 0, 1, 604800)
            .AddProduct(99, 157, 80, 14, 92, 0, 1, 0)
            .AddProduct(100, 158, 80, 14, 92, 0, 1, 0);
        this.AddPackage(configuration, 163, LocalizedString.FromResource(() => CashShopPackageNames.BracketedGladiatorPackage), 500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(115, 174, 0, 14, 72, 0, 1, 86400)
            .AddProduct(116, 175, 0, 14, 133, 0, 10, 0)
            .AddProduct(118, 177, 130, 13, 107, 0, 1, 86400)
            .AddProduct(119, 178, 100, 13, 64, 0, 1, 86400)
            .AddProduct(120, 179, 150, 14, 98, 0, 1, 86400)
            .AddProduct(121, 180, 120, 13, 105, 0, 1, 86400)
            .AddProduct(122, 181, 0, 13, 113, 0, 1, 86400);
        this.AddPackage(configuration, 13, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSilverKey), 20, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(69, 117, 20, 14, 112, 0, 1, 0);
        this.AddPackage(configuration, 12, LocalizedString.FromResource(() => CashShopPackageNames.BracketedGoldKey), 60, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(70, 118, 60, 14, 113, 0, 1, 0);
        this.AddPackage(configuration, 99, LocalizedString.FromResource(() => CashShopPackageNames.BracketedTalismanofLuck), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(22, 37, 100, 14, 53, 0, 1, 0)
            .AddProduct(22, 38, 400, 14, 53, 0, 1, 0);
        this.AddPackage(configuration, 264, LocalizedString.FromResource(() => CashShopPackageNames.BracketedRageFighterCharacterCard), 500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(284, 353, 500, 14, 169, 0, 1, 0);
        this.AddPackage(configuration, 127, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSummonerCharacterCard), 500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(31, 53, 500, 14, 91, 0, 1, 0);
        this.AddPackage(configuration, 115, LocalizedString.FromResource(() => CashShopPackageNames.BracketedResetFruitStrength), 250, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(14, 24, 250, 13, 54, 0, 1, 0);
        this.AddPackage(configuration, 113, LocalizedString.FromResource(() => CashShopPackageNames.BracketedResetFruitQuickness), 250, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(15, 25, 250, 13, 55, 0, 1, 0);
        this.AddPackage(configuration, 111, LocalizedString.FromResource(() => CashShopPackageNames.BracketedResetFruitHealth), 250, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(16, 26, 250, 13, 56, 0, 1, 0);
        this.AddPackage(configuration, 109, LocalizedString.FromResource(() => CashShopPackageNames.BracketedResetFruitEnergy), 250, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(17, 27, 250, 13, 57, 0, 1, 0);
        this.AddPackage(configuration, 107, LocalizedString.FromResource(() => CashShopPackageNames.BracketedResetFruitControl), 250, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(18, 28, 250, 13, 58, 0, 1, 0);
        this.AddPackage(configuration, 2052, LocalizedString.FromResource(() => CashShopPackageNames.BlueGoldChannelPackage), 600, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(912, 1113, 600, 13, 124, 0, 1, 2592000)
            .AddProduct(913, 1114, 480, 13, 124, 0, 1, 604800);
        this.AddPackage(configuration, 71, LocalizedString.FromResource(() => CashShopPackageNames.BracketedTalismanofResurrection), 50, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(40, 66, 50, 13, 69, 0, 1, 0);
        this.AddPackage(configuration, 69, LocalizedString.FromResource(() => CashShopPackageNames.BracketedTalismanofMobility), 40, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(41, 67, 40, 13, 70, 0, 1, 0);
        this.AddPackage(configuration, 440, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPSealofWealth7Day), 700, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(511, 590, 0, 14, 120, 0, 1, 0)
            .AddProduct(518, 597, 700, 13, 44, 0, 1, 604800);
        this.AddPackage(configuration, 438, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPSealofWealth30Day), 2100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(516, 595, 0, 14, 120, 0, 1, 0)
            .AddProduct(519, 598, 2100, 13, 44, 0, 1, 2592000);
        this.AddPackage(configuration, 436, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPMasterSealofWealth7Day), 700, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(511, 590, 0, 14, 120, 0, 1, 0)
            .AddProduct(520, 599, 700, 13, 94, 0, 1, 604800);
        this.AddPackage(configuration, 434, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPMasterSealofWealth30Day), 2100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(516, 595, 0, 14, 120, 0, 1, 0)
            .AddProduct(521, 600, 2100, 13, 94, 0, 1, 2592000);
        this.AddPackage(configuration, 432, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPSealofAscension7Day), 500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(509, 588, 0, 14, 120, 0, 1, 0)
            .AddProduct(522, 601, 500, 13, 43, 0, 1, 604800);
        this.AddPackage(configuration, 430, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPSealofAscension30Day), 1400, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(514, 593, 0, 14, 120, 0, 1, 0)
            .AddProduct(529, 611, 1400, 13, 43, 0, 1, 2592000);
        this.AddPackage(configuration, 428, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPMasterSealofAscension7Day), 500, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(509, 588, 0, 14, 120, 0, 1, 0)
            .AddProduct(524, 603, 500, 13, 93, 0, 1, 604800);
        this.AddPackage(configuration, 426, LocalizedString.FromResource(() => CashShopPackageNames.BonusGPMasterSealofAscension30Day), 1400, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(514, 593, 0, 14, 120, 0, 1, 0)
            .AddProduct(525, 604, 1400, 13, 93, 0, 1, 2592000);
        this.AddPackage(configuration, 123, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSealofWealth), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(10, 13, 150, 13, 44, 0, 1, 86400)
            .AddProduct(10, 14, 350, 13, 44, 0, 1, 259200)
            .AddProduct(10, 15, 700, 13, 44, 0, 1, 604800)
            .AddProduct(10, 16, 2100, 13, 44, 0, 1, 2592000);
        this.AddPackage(configuration, 125, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSealofAscension), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(9, 9, 100, 13, 43, 0, 1, 86400)
            .AddProduct(9, 10, 250, 13, 43, 0, 1, 259200)
            .AddProduct(9, 11, 500, 13, 43, 0, 1, 604800)
            .AddProduct(9, 12, 1400, 13, 43, 0, 1, 2592000);
        this.AddPackage(configuration, 49, LocalizedString.FromResource(() => CashShopPackageNames.BracketedMasterSealofWealth), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(51, 80, 150, 13, 94, 0, 1, 86400)
            .AddProduct(51, 81, 700, 13, 94, 0, 1, 604800)
            .AddProduct(51, 82, 2100, 13, 94, 0, 1, 2592000);
        this.AddPackage(configuration, 51, LocalizedString.FromResource(() => CashShopPackageNames.BracketedMasterSealofAscension), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(50, 77, 100, 13, 93, 0, 1, 86400)
            .AddProduct(50, 78, 500, 13, 93, 0, 1, 604800)
            .AddProduct(50, 79, 1400, 13, 93, 0, 1, 2592000);
        this.AddPackage(configuration, 121, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSealofSustenance), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(11, 17, 80, 13, 45, 0, 1, 86400)
            .AddProduct(11, 18, 680, 13, 45, 0, 1, 864000)
            .AddProduct(11, 19, 1250, 13, 45, 0, 1, 2592000);
        this.AddPackage(configuration, 75, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSealofHealing), 120, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(38, 62, 120, 13, 62, 0, 1, 86400)
            .AddProduct(38, 63, 600, 13, 62, 0, 1, 604800);
        this.AddPackage(configuration, 73, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSealofDivinity), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(39, 64, 100, 13, 63, 0, 1, 86400)
            .AddProduct(39, 65, 500, 13, 63, 0, 1, 604800);
        this.AddPackage(configuration, 8, LocalizedString.FromResource(() => CashShopPackageNames.BracketedAGBoostAura), 120, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(73, 123, 120, 13, 104, 0, 1, 86400)
            .AddProduct(73, 124, 600, 13, 104, 0, 1, 604800);
        this.AddPackage(configuration, 7, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSDBoostAura), 120, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(74, 125, 120, 13, 105, 0, 1, 86400)
            .AddProduct(74, 126, 600, 13, 105, 0, 1, 604800);
        this.AddPackage(configuration, 357, LocalizedString.FromResource(() => CashShopPackageNames.BracketedWornHorseshoe), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(463, 540, 80, 13, 134, 0, 1, 86400)
            .AddProduct(463, 541, 400, 13, 134, 0, 1, 604800)
            .AddProduct(463, 542, 1200, 13, 134, 0, 1, 2592000);
        this.AddPackage(configuration, 355, LocalizedString.FromResource(() => CashShopPackageNames.BracketedGoldenOakCharm), 120, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(462, 537, 120, 13, 132, 0, 1, 86400)
            .AddProduct(462, 538, 600, 13, 132, 0, 1, 604800)
            .AddProduct(462, 539, 1800, 13, 132, 0, 1, 2592000);
        this.AddPackage(configuration, 353, LocalizedString.FromResource(() => CashShopPackageNames.BracketedOakCharm), 30, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(461, 534, 30, 13, 130, 0, 1, 86400)
            .AddProduct(461, 535, 150, 13, 130, 0, 1, 604800)
            .AddProduct(461, 536, 500, 13, 130, 0, 1, 2592000);
        this.AddPackage(configuration, 349, LocalizedString.FromResource(() => CashShopPackageNames.BracketedStatueofHawk), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(459, 528, 80, 13, 128, 0, 1, 86400)
            .AddProduct(459, 529, 400, 13, 128, 0, 1, 604800)
            .AddProduct(459, 530, 1200, 13, 128, 0, 1, 2592000);
        this.AddPackage(configuration, 351, LocalizedString.FromResource(() => CashShopPackageNames.BracketedStatueofGoral), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(460, 531, 80, 13, 129, 0, 1, 86400)
            .AddProduct(460, 532, 400, 13, 129, 0, 1, 604800)
            .AddProduct(460, 533, 1200, 13, 129, 0, 1, 2592000);
        this.AddPackage(configuration, 196, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollPackageA), 320, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(159, 219, 120, 14, 74, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400);
        this.AddPackage(configuration, 198, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollPackageB), 320, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(158, 218, 120, 14, 75, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400);
        this.AddPackage(configuration, 200, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollPackageC), 488, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(156, 216, 56, 14, 77, 0, 1, 86400)
            .AddProduct(157, 217, 56, 14, 76, 0, 1, 86400)
            .AddProduct(159, 219, 120, 14, 74, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400)
            .AddProduct(162, 222, 56, 14, 97, 0, 1, 86400);
        this.AddPackage(configuration, 202, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollPackageD), 488, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(156, 216, 56, 14, 77, 0, 1, 86400)
            .AddProduct(157, 217, 56, 14, 76, 0, 1, 86400)
            .AddProduct(158, 218, 120, 14, 75, 0, 1, 86400)
            .AddProduct(160, 220, 100, 14, 73, 0, 1, 86400)
            .AddProduct(161, 221, 100, 14, 72, 0, 1, 86400)
            .AddProduct(162, 222, 56, 14, 97, 0, 1, 86400);
        this.AddPackage(configuration, 91, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofProtection), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(26, 43, 100, 14, 73, 0, 1, 86400)
            .AddProduct(26, 44, 500, 14, 73, 0, 1, 604800);
        this.AddPackage(configuration, 93, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofQuickness), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(25, 41, 150, 14, 72, 0, 1, 86400)
            .AddProduct(25, 42, 700, 14, 72, 0, 1, 604800);
        this.AddPackage(configuration, 89, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofWrath), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(27, 45, 150, 14, 74, 0, 1, 86400)
            .AddProduct(27, 46, 700, 14, 74, 0, 1, 604800);
        this.AddPackage(configuration, 87, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofWizardry), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(28, 47, 150, 14, 75, 0, 1, 86400)
            .AddProduct(28, 48, 700, 14, 75, 0, 1, 604800);
        this.AddPackage(configuration, 77, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofStrengthener), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(37, 60, 150, 14, 98, 0, 1, 86400)
            .AddProduct(37, 61, 700, 14, 98, 0, 1, 604800);
        this.AddPackage(configuration, 79, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofBattle), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(36, 58, 150, 14, 97, 0, 1, 86400)
            .AddProduct(36, 59, 700, 14, 97, 0, 1, 604800);
        this.AddPackage(configuration, 83, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofMana), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(30, 51, 100, 14, 77, 0, 1, 86400)
            .AddProduct(30, 52, 500, 14, 77, 0, 1, 604800);
        this.AddPackage(configuration, 85, LocalizedString.FromResource(() => CashShopPackageNames.BracketedScrollofHealth), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(29, 49, 100, 14, 76, 0, 1, 86400)
            .AddProduct(29, 50, 500, 14, 76, 0, 1, 604800);
        this.AddPackage(configuration, 31, LocalizedString.FromResource(() => CashShopPackageNames.BracketedPetPanda), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(60, 99, 150, 13, 80, 0, 1, 86400)
            .AddProduct(60, 100, 700, 13, 80, 0, 1, 604800);
        this.AddPackage(configuration, 29, LocalizedString.FromResource(() => CashShopPackageNames.BracketedPetSkeleton), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(61, 101, 150, 13, 123, 0, 1, 86400)
            .AddProduct(61, 102, 700, 13, 123, 0, 1, 604800);
        this.AddPackage(configuration, 11, LocalizedString.FromResource(() => CashShopPackageNames.BracketedPetUnicorn), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(72, 121, 150, 13, 106, 0, 1, 86400)
            .AddProduct(72, 122, 700, 13, 106, 0, 1, 604800);
        this.AddPackage(configuration, 103, LocalizedString.FromResource(() => CashShopPackageNames.BracketedDemon), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(20, 31, 100, 13, 64, 0, 1, 86400)
            .AddProduct(20, 32, 500, 13, 64, 0, 1, 604800)
            .AddProduct(20, 33, 1400, 13, 64, 0, 1, 2592000);
        this.AddPackage(configuration, 101, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSpiritofGuardian), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(21, 34, 100, 13, 65, 0, 1, 86400)
            .AddProduct(21, 35, 500, 13, 65, 0, 1, 604800)
            .AddProduct(21, 36, 1400, 13, 65, 0, 1, 2592000);
        this.AddPackage(configuration, 37, LocalizedString.FromResource(() => CashShopPackageNames.BracketedPandaRing), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(57, 93, 100, 13, 76, 0, 1, 86400)
            .AddProduct(57, 94, 500, 13, 76, 0, 1, 604800);
        this.AddPackage(configuration, 33, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSkeletonTransformationRing), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(59, 97, 100, 13, 122, 0, 1, 86400)
            .AddProduct(59, 98, 500, 13, 122, 0, 1, 604800);
        this.AddPackage(configuration, 35, LocalizedString.FromResource(() => CashShopPackageNames.BracketedWizardRing), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(58, 95, 100, 13, 20, 0, 1, 86400)
            .AddProduct(58, 96, 500, 13, 20, 0, 1, 604800);
        this.AddPackage(configuration, 148, LocalizedString.FromResource(() => CashShopPackageNames.BracketedCriticalRingofWizard), 130, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(71, 119, 130, 13, 107, 0, 1, 86400)
            .AddProduct(71, 120, 600, 13, 107, 0, 1, 604800);
        this.AddPackage(configuration, 27, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSapphireRing), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(62, 103, 80, 13, 109, 0, 1, 86400)
            .AddProduct(62, 104, 400, 13, 109, 0, 1, 604800);
        this.AddPackage(configuration, 25, LocalizedString.FromResource(() => CashShopPackageNames.BracketedRubyRing), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(63, 105, 80, 13, 110, 0, 1, 86400)
            .AddProduct(63, 106, 400, 13, 110, 0, 1, 604800);
        this.AddPackage(configuration, 23, LocalizedString.FromResource(() => CashShopPackageNames.BracketedTopazRing), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(64, 107, 80, 13, 111, 0, 1, 86400)
            .AddProduct(64, 108, 400, 13, 111, 0, 1, 604800);
        this.AddPackage(configuration, 21, LocalizedString.FromResource(() => CashShopPackageNames.BracketedAmethystRing), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(65, 109, 80, 13, 112, 0, 1, 86400)
            .AddProduct(65, 110, 400, 13, 112, 0, 1, 604800);
        this.AddPackage(configuration, 19, LocalizedString.FromResource(() => CashShopPackageNames.BracketedRubyNecklace), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(66, 111, 90, 13, 113, 0, 1, 86400)
            .AddProduct(66, 112, 450, 13, 113, 0, 1, 604800);
        this.AddPackage(configuration, 17, LocalizedString.FromResource(() => CashShopPackageNames.BracketedEmeraldNecklace), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(67, 113, 90, 13, 114, 0, 1, 86400)
            .AddProduct(67, 114, 450, 13, 114, 0, 1, 604800);
        this.AddPackage(configuration, 15, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSapphireNecklace), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(68, 115, 90, 13, 115, 0, 1, 86400)
            .AddProduct(68, 116, 450, 13, 115, 0, 1, 604800);
        this.AddPackage(configuration, 257, LocalizedString.FromResource(() => CashShopPackageNames.BracketedLittleWarriorsCloak), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(283, 351, 90, 12, 135, 0, 1, 86400)
            .AddProduct(283, 352, 450, 12, 135, 0, 1, 604800);
        this.AddPackage(configuration, 45, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSmallWingsofDisaster), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(53, 85, 90, 12, 131, 0, 1, 86400)
            .AddProduct(53, 86, 450, 12, 131, 0, 1, 604800);
        this.AddPackage(configuration, 43, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSmallWingsofFairy), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(54, 87, 90, 12, 132, 0, 1, 86400)
            .AddProduct(54, 88, 450, 12, 132, 0, 1, 604800);
        this.AddPackage(configuration, 41, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSmallWingsofHeaven), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(55, 89, 90, 12, 133, 0, 1, 86400)
            .AddProduct(55, 90, 450, 12, 133, 0, 1, 604800);
        this.AddPackage(configuration, 39, LocalizedString.FromResource(() => CashShopPackageNames.BracketedSmallWingsofSatan), 90, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(56, 91, 90, 12, 134, 0, 1, 86400)
            .AddProduct(56, 92, 450, 12, 134, 0, 1, 604800);
        this.AddPackage(configuration, 2051, LocalizedString.FromResource(() => CashShopPackageNames.PlatinumGoldChannelPackage), 1600, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: true)
            .AddProduct(914, 1115, 480, 13, 124, 0, 1, 1296000)
            .AddProduct(915, 1116, 1600, 13, 124, 0, 1, 2592000);
        this.AddPackage(configuration, 360, LocalizedString.FromResource(() => CashShopPackageNames.FirstLuckyArmorTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(465, 544, 100, 13, 135, 0, 1, 0);
        this.AddPackage(configuration, 361, LocalizedString.FromResource(() => CashShopPackageNames.FirstLuckyPantsTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(466, 545, 100, 13, 136, 0, 1, 0);
        this.AddPackage(configuration, 362, LocalizedString.FromResource(() => CashShopPackageNames.FirstLuckyHelmTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(467, 546, 100, 13, 137, 0, 1, 0);
        this.AddPackage(configuration, 363, LocalizedString.FromResource(() => CashShopPackageNames.FirstLuckyGlovesTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(468, 547, 100, 13, 138, 0, 1, 0);
        this.AddPackage(configuration, 364, LocalizedString.FromResource(() => CashShopPackageNames.FirstLuckyBootsTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(469, 548, 100, 13, 139, 0, 1, 0);
        this.AddPackage(configuration, 365, LocalizedString.FromResource(() => CashShopPackageNames.SecondLuckyArmorTicket), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(470, 549, 150, 13, 140, 0, 1, 0);
        this.AddPackage(configuration, 366, LocalizedString.FromResource(() => CashShopPackageNames.SecondLuckyPantsTicket), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(471, 550, 150, 13, 141, 0, 1, 0);
        this.AddPackage(configuration, 367, LocalizedString.FromResource(() => CashShopPackageNames.SecondLuckyHelmTicket), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(472, 551, 150, 13, 142, 0, 1, 0);
        this.AddPackage(configuration, 368, LocalizedString.FromResource(() => CashShopPackageNames.SecondLuckyGlovesTicket), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(473, 552, 150, 13, 143, 0, 1, 0);
        this.AddPackage(configuration, 369, LocalizedString.FromResource(() => CashShopPackageNames.SecondLuckyBootsTicket), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(474, 553, 150, 13, 144, 0, 1, 0);
        this.AddPackage(configuration, 119, LocalizedString.FromResource(() => CashShopPackageNames.BracketedDevilSquareTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(12, 20, 100, 13, 46, 0, 1, 0)
            .AddProduct(12, 21, 900, 13, 46, 0, 10, 0);
        this.AddPackage(configuration, 117, LocalizedString.FromResource(() => CashShopPackageNames.BracketedBloodCastleTicket), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(13, 22, 100, 13, 47, 0, 1, 0)
            .AddProduct(13, 23, 900, 13, 47, 0, 10, 0);
        this.AddPackage(configuration, 105, LocalizedString.FromResource(() => CashShopPackageNames.BracketedIllusionTempleTicket), 150, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(19, 29, 150, 13, 61, 0, 1, 0)
            .AddProduct(19, 30, 1350, 13, 61, 0, 10, 0);
        this.AddPackage(configuration, 57, LocalizedString.FromResource(() => CashShopPackageNames.BracketedKalimaTicket), 80, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(47, 73, 80, 13, 48, 0, 1, 0)
            .AddProduct(47, 74, 720, 13, 48, 0, 10, 0);
        this.AddPackage(configuration, 149, LocalizedString.FromResource(() => CashShopPackageNames.BracketedOpenAccessTickettoVarka7), 100, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(79, 134, 100, 13, 127, 0, 1, 0)
            .AddProduct(79, 135, 900, 13, 127, 0, 10, 0);
        this.AddPackage(configuration, 5, LocalizedString.FromResource(() => CashShopPackageNames.BracketedEliteSDPotion), 40, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(76, 129, 40, 14, 133, 0, 10, 0);
        this.AddPackage(configuration, 97, LocalizedString.FromResource(() => CashShopPackageNames.BracketedEliteHealingPotion), 20, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(23, 39, 20, 14, 70, 0, 20, 0);
        this.AddPackage(configuration, 95, LocalizedString.FromResource(() => CashShopPackageNames.BracketedEliteManaPotion), 20, CashShopCoinType.WCoinP, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(24, 40, 20, 14, 71, 0, 20, 0);
        this.AddPackage(configuration, 385, LocalizedString.FromResource(() => CashShopPackageNames.BundleofJewelofSoul30), 2000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(500, 579, 2000, 12, 31, 2, 1, 0);
        this.AddPackage(configuration, 384, LocalizedString.FromResource(() => CashShopPackageNames.BundleofJewelofBless30), 2000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(499, 578, 2000, 12, 30, 2, 1, 0);
        this.AddPackage(configuration, 386, LocalizedString.FromResource(() => CashShopPackageNames.JewelofChaosBundle10), 2000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(501, 580, 2000, 12, 141, 0, 1, 0);
        this.AddPackage(configuration, 387, LocalizedString.FromResource(() => CashShopPackageNames.JewelofLifeBundle10), 4800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(502, 581, 4800, 12, 136, 0, 1, 0);
        this.AddPackage(configuration, 388, LocalizedString.FromResource(() => CashShopPackageNames.BundleofJewelofHarmony10), 4800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(503, 582, 4800, 12, 140, 0, 1, 0);
        this.AddPackage(configuration, 389, LocalizedString.FromResource(() => CashShopPackageNames.TalismanofLuck10Percent), 3200, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(504, 583, 3200, 14, 53, 0, 1, 0);
        this.AddPackage(configuration, 391, LocalizedString.FromResource(() => CashShopPackageNames.RareItemTicket8), 100000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(506, 585, 100000, 14, 146, 0, 1, 0);
        this.AddPackage(configuration, 390, LocalizedString.FromResource(() => CashShopPackageNames.RareItemTicket11), 60000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(505, 584, 60000, 14, 149, 0, 1, 0);
        this.AddPackage(configuration, 465, LocalizedString.FromResource(() => CashShopPackageNames.GPBoxofkundunPlus5), 3200, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(563, 650, 3200, 14, 11, 12, 1, 0);
        this.AddPackage(configuration, 464, LocalizedString.FromResource(() => CashShopPackageNames.GPBoxofkundunPlus4), 2400, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(562, 649, 2400, 14, 11, 11, 1, 0);
        this.AddPackage(configuration, 463, LocalizedString.FromResource(() => CashShopPackageNames.GPBoxofkundunPlus3), 1600, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(561, 648, 1600, 14, 11, 10, 1, 0);
        this.AddPackage(configuration, 462, LocalizedString.FromResource(() => CashShopPackageNames.GPBoxofkundunPlus2), 800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(560, 647, 800, 14, 11, 9, 1, 0);
        this.AddPackage(configuration, 461, LocalizedString.FromResource(() => CashShopPackageNames.GPBoxofKundunPlus1), 400, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: false, isBundle: false)
            .AddProduct(559, 646, 400, 14, 11, 8, 1, 0);
        this.AddPackage(configuration, 373, LocalizedString.FromResource(() => CashShopPackageNames.CherryBlossomPetal), 200, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(488, 567, 200, 14, 87, 0, 1, 0);
        this.AddPackage(configuration, 375, LocalizedString.FromResource(() => CashShopPackageNames.CherryBlossomRiceCake), 300, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(490, 569, 300, 14, 86, 0, 1, 0);
        this.AddPackage(configuration, 374, LocalizedString.FromResource(() => CashShopPackageNames.CherryBlossomWine), 300, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(489, 568, 300, 14, 85, 0, 1, 0);
        this.AddPackage(configuration, 376, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofProtection7Day), 2000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(491, 570, 2000, 14, 73, 0, 1, 604800);
        this.AddPackage(configuration, 378, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofMana7Day), 2000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(493, 572, 2000, 14, 77, 0, 1, 604800);
        this.AddPackage(configuration, 377, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofHealth7Day), 2000, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(492, 571, 2000, 14, 76, 0, 1, 604800);
        this.AddPackage(configuration, 382, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofWrath7Day), 2800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(497, 576, 2800, 14, 74, 0, 1, 604800);
        this.AddPackage(configuration, 383, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofWizardry7Day), 2800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(498, 577, 2800, 14, 75, 0, 1, 604800);
        this.AddPackage(configuration, 381, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofQuickness7Day), 2800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(496, 575, 2800, 14, 72, 0, 1, 604800);
        this.AddPackage(configuration, 379, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofBattle7Day), 2800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(494, 573, 2800, 14, 97, 0, 1, 604800);
        this.AddPackage(configuration, 380, LocalizedString.FromResource(() => CashShopPackageNames.ScrollofStrengthener7Day), 2800, CashShopCoinType.GoblinPoints, isForSale: true, isGiftable: true, isBundle: false)
            .AddProduct(495, 574, 2800, 14, 98, 0, 1, 604800);
    }

    private PackageBuilder AddPackage(CashShopConfiguration configuration, int packageSequence, LocalizedString name, int price, CashShopCoinType coinType, bool isForSale, bool isGiftable, bool isBundle)
    {
        var package = this.Context.CreateNew<CashShopPackage>();
        package.PackageSequence = packageSequence;
        package.Name = name;
        package.Price = price;
        package.CoinType = coinType;
        package.IsForSale = isForSale;
        package.IsGiftable = isGiftable;
        package.IsBundle = isBundle;
        configuration.Packages.Add(package);
        return new PackageBuilder(this, package);
    }

    /// <summary>
    /// Adds the products to a package.
    /// </summary>
    private sealed class PackageBuilder
    {
        private readonly CashShopInitializer _initializer;
        private readonly CashShopPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageBuilder"/> class.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <param name="package">The package.</param>
        public PackageBuilder(CashShopInitializer initializer, CashShopPackage package)
        {
            this._initializer = initializer;
            this._package = package;
        }

        /// <summary>
        /// Adds a product to the package.
        /// </summary>
        /// <param name="productSequence">The product sequence number of the script.</param>
        /// <param name="priceSequence">The price sequence number of the script.</param>
        /// <param name="price">The price.</param>
        /// <param name="itemGroup">The group of the item.</param>
        /// <param name="itemNumber">The number of the item.</param>
        /// <param name="itemLevel">The level of the item.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="durationSeconds">The duration in seconds; 0, if the item isn't limited.</param>
        /// <returns>This builder.</returns>
        public PackageBuilder AddProduct(int productSequence, int priceSequence, int price, byte itemGroup, short itemNumber, byte itemLevel, int quantity, int durationSeconds)
        {
            var product = this._initializer.Context.CreateNew<CashShopProduct>();
            product.ProductSequence = productSequence;
            product.PriceSequence = priceSequence;
            product.Price = price;
            product.ItemDefinition = this._initializer.GameConfiguration.Items.FirstOrDefault(item => item.Group == itemGroup && item.Number == itemNumber);
            product.ItemLevel = itemLevel;
            product.Quantity = quantity;
            product.Duration = TimeSpan.FromSeconds(durationSeconds);
            this._package.Products.Add(product);
            if (!product.CanBeDelivered())
            {
                this._package.IsForSale = false;
            }

            return this;
        }
    }
}
