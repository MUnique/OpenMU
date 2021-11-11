// <copyright file="ChaosMixes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Version075.Items;

/// <summary>
/// Initializer for chaos mixes.
/// </summary>
public class ChaosMixes : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosMixes"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ChaosMixes(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var chaosGoblin = this.GameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);
        chaosGoblin.ItemCraftings.Add(this.ChaosWeaponCrafting());
    }

    private ItemCrafting ChaosWeaponCrafting()
    {
        var chaosWeapon = this.Context.CreateNew<ItemCrafting>();
        chaosWeapon.Name = "Chaos Weapon";
        chaosWeapon.Number = 1;
        var chaosWeaponSettings = this.Context.CreateNew<SimpleCraftingSettings>();

        chaosWeapon.SimpleCraftingSettings = chaosWeaponSettings;
        chaosWeaponSettings.MoneyPerFinalSuccessPercentage = 10000;
        chaosWeaponSettings.SuccessPercent = 1;

        // Requirements:
        var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
        randomItem.MinimumAmount = 1;
        randomItem.MinimumItemLevel = 4;
        randomItem.MaximumItemLevel = Constants.MaximumItemLevel;
        randomItem.NpcPriceDivisor = 15000;
        randomItem.FailResult = MixResult.DowngradedRandom;
        randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
        randomItem.SuccessResult = MixResult.Disappear;
        chaosWeaponSettings.RequiredItems.Add(randomItem);

        var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
        chaos.MinimumAmount = 1;
        chaos.AddPercentage = 2;
        chaos.SuccessResult = MixResult.Disappear;
        chaos.FailResult = MixResult.Disappear;
        chaos.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        chaosWeaponSettings.RequiredItems.Add(chaos);

        var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
        bless.MinimumAmount = 0;
        bless.AddPercentage = 5;
        bless.SuccessResult = MixResult.Disappear;
        bless.FailResult = MixResult.Disappear;
        bless.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless"));
        chaosWeaponSettings.RequiredItems.Add(bless);

        var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
        soul.MinimumAmount = 0;
        soul.AddPercentage = 4;
        soul.SuccessResult = MixResult.Disappear;
        soul.FailResult = MixResult.Disappear;
        soul.PossibleItems.Add(this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul"));
        chaosWeaponSettings.RequiredItems.Add(soul);

        // Result:
        chaosWeaponSettings.ResultItemSelect = ResultItemSelection.Any;
        chaosWeaponSettings.ResultItemLuckOptionChance = 10;
        chaosWeaponSettings.ResultItemSkillChance = 30;

        var chaosDragonAxe = this.Context.CreateNew<ItemCraftingResultItem>();
        chaosDragonAxe.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Dragon Axe");
        chaosWeaponSettings.ResultItems.Add(chaosDragonAxe);

        var chaosNatureBow = this.Context.CreateNew<ItemCraftingResultItem>();
        chaosNatureBow.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Nature Bow");
        chaosWeaponSettings.ResultItems.Add(chaosNatureBow);

        var chaosLightningStaff = this.Context.CreateNew<ItemCraftingResultItem>();
        chaosLightningStaff.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Lightning Staff");
        chaosWeaponSettings.ResultItems.Add(chaosLightningStaff);

        return chaosWeapon;
    }
}