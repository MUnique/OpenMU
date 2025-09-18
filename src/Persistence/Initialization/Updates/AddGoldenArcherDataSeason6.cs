// Adds Golden Archer essentials: Rena item and a default reward drop group

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Persistence.Json;
using MUnique.OpenMU.GameLogic.PlugIns.GoldenArcher;

[PlugIn(PlugInName, PlugInDescription)]
[Guid("F4C9D445-C7B4-4CC8-A3A0-C0A5E2B2E3A1")]
public class AddGoldenArcherDataSeason6 : UpdatePlugInBase
{
    internal const string PlugInName = "Golden Archer: Rena + Reward Group";
    internal const string PlugInDescription = "Adds Rena item (group 14, number 21) and a default 'Golden Archer Rewards' drop group with common jewels.";

    private const short RenaGroup = 14;
    private const short RenaNumber = 21; // Typical client mapping for Rena
    private const short RewardDropId = 32001;

    public override UpdateVersion Version => UpdateVersion.GoldenArcherDataSeason6;

    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    public override string Name => PlugInName;

    public override string Description => PlugInDescription;

    public override bool IsMandatory => false;

    public override DateTime CreatedAt => new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        EnsureRenaItem(context, gameConfiguration);
        EnsureRewardDropGroup(context, gameConfiguration);
        EnsurePackedJewelDropGroup(context, gameConfiguration);
        EnsureGoldenArcherPluginConfigured(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }

    private static void EnsureRenaItem(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Items.Any(i => i.Group == RenaGroup && i.Number == RenaNumber)
            || gameConfiguration.Items.Any(i => string.Equals(i.Name, "Rena", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var itemDefinition = context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Rena";
        itemDefinition.Number = RenaNumber;
        itemDefinition.Group = (byte)RenaGroup;
        itemDefinition.DropsFromMonsters = false; // event token
        itemDefinition.Durability = 1;
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        gameConfiguration.Items.Add(itemDefinition);
    }

    private static void EnsureRewardDropGroup(IContext context, GameConfiguration gameConfiguration)
    {
        var id = GuidHelper.CreateGuid<DropItemGroup>(RewardDropId);
        if (gameConfiguration.DropItemGroups.Any(g => g.GetId() == id))
        {
            return;
        }

        var drop = context.CreateNew<DropItemGroup>();
        drop.SetGuid(RewardDropId);
        drop.Description = "Golden Archer Rewards";
        drop.Chance = 1.0; // not relevant for direct generation, but ok as default

        // Add common jewels found in S6 configs
        string[] jewelNames =
        {
            "Jewel of Bless",
            "Jewel of Soul",
            "Jewel of Chaos",
            "Jewel of Life",
            "Jewel of Creation",
        };

        foreach (var name in jewelNames)
        {
            var item = gameConfiguration.Items.FirstOrDefault(i => i.Name == name);
            if (item is not null && !drop.PossibleItems.Contains(item))
            {
                // Ensure these are not general monster drops by default
                if (item.DropsFromMonsters)
                {
                    item.DropsFromMonsters = false;
                }

                drop.PossibleItems.Add(item);
            }
        }

        gameConfiguration.DropItemGroups.Add(drop);
    }

    private static void EnsurePackedJewelDropGroup(IContext context, GameConfiguration gameConfiguration)
    {
        const short packedId = 32002;
        var id = GuidHelper.CreateGuid<DropItemGroup>(packedId);
        if (gameConfiguration.DropItemGroups.Any(g => g.GetId() == id))
        {
            return;
        }

        var drop = context.CreateNew<DropItemGroup>();
        drop.SetGuid(packedId);
        drop.Description = "Golden Archer Packed Jewels";
        drop.Chance = 1.0;

        string[] packed =
        {
            "Packed Jewel of Bless",
            "Packed Jewel of Soul",
            "Packed Jewel of Life",
            "Packed Jewel of Creation",
            "Packed Jewel of Chaos",
        };

        foreach (var name in packed)
        {
            var item = gameConfiguration.Items.FirstOrDefault(i => i.Name == name);
            if (item is not null)
            {
                drop.PossibleItems.Add(item);
            }
        }

        gameConfiguration.DropItemGroups.Add(drop);
    }

    private static void EnsureGoldenArcherPluginConfigured(IContext context, GameConfiguration gameConfiguration)
    {
        var token = gameConfiguration.Items.FirstOrDefault(i => i.Group == RenaGroup && i.Number == RenaNumber)
                    ?? gameConfiguration.Items.FirstOrDefault(i => string.Equals(i.Name, "Rena", StringComparison.OrdinalIgnoreCase));
        var reward = gameConfiguration.DropItemGroups.FirstOrDefault(g => g.Description == "Golden Archer Rewards");
        var packed = gameConfiguration.DropItemGroups.FirstOrDefault(g => g.Description == "Golden Archer Packed Jewels");
        if (token is null || reward is null || packed is null)
        {
            return;
        }

        var typeId = typeof(GoldenArcherNpcPlugIn).GUID;
        var plug = gameConfiguration.PlugInConfigurations.FirstOrDefault(p => p.TypeId == typeId)
                   ?? context.CreateNew<PlugInConfiguration>();

        if (!gameConfiguration.PlugInConfigurations.Contains(plug))
        {
            plug.TypeId = typeId;
            gameConfiguration.PlugInConfigurations.Add(plug);
        }

        plug.IsActive = true;

        var cfg = new GoldenArcherNpcPlugInConfiguration
        {
            TokenItemDefinition = token,
            TokenItemGroup = 14,
            TokenItemNumber = 21,
            MaximumAcceptedTokens = 255,
            LowTierMoneyPerToken = 10000,
            MidTierItemGroup = 14, // Box of Luck group
            MidTierItemNumber = 11, // Box of Luck number
            MidTierItemLevel = 7,   // => Box of Heaven
            HighTierDropGroupDescription = reward.Description,
            AdvancedTierDropGroupDescription = packed.Description,
            TopTierExact = 255,
            TopTierItemGroup = 14,
            TopTierItemNumber = 11,
            TopTierItemLevel = 12, // => Box of Kundun +5
        };

        // Store with id-based references so the game server can resolve them by data source later.
        plug.SetConfiguration(cfg, new IdReferenceHandler());
    }
}
