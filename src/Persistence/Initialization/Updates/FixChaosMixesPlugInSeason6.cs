// <copyright file="FixChaosMixesPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the Chaos Weapon, First Wings, Dinorant, Item Level Upgrade, Second Wings, Third Wings, Cape, SD Potions, Guardian Option, and Secromicon crafting settings; Blue Fenrir (Protect) damage decrease option value; Wizard's Ring wizardry option; lvl 380 item guardian options for Summoner and Rage Fighter.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("EFD7EA69-56AE-48A3-ACE2-1C3B5B87780A")]
public class FixChaosMixesPlugInSeason6 : FixChaosMixesPlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    private new const string PlugInName = "Fix Chaos Mixes Settings And Several Options";

    /// <summary>
    /// The plug in description.
    /// </summary>
    private new const string PlugInDescription = "This update fixes the Chaos Weapon, First Wings, Dinorant, Item Level Upgrade, Second Wings, Third Wings, Cape, SD Potions, Guardian Option, and Secromicon crafting settings; Blue Fenrir (Protect) damage decrease option value; Wizard's Ring wizardry option; lvl 380 item guardian options for Summoner and Rage Fighter.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixChaosMixesSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Dark horse spirit and raven spirit drop item groups id fix (do this first because it persists changes to DB)
        if (gameConfiguration.Items.Single(id => id.Name == "Spirit") is { } spirit)
        {
            spirit.MaximumItemLevel = 1;
            var maps = gameConfiguration.Maps;
            if (gameConfiguration.DropItemGroups.Single(dig => dig.Description == "Dark Horse Spirit") is { } oldHorseGroup
                && gameConfiguration.DropItemGroups.Single(dig => dig.Description == "Dark Raven Spirit") is { } oldRavenGroup)
            {
                DeleteDropItemGroup(oldHorseGroup);
                DeleteDropItemGroup(oldRavenGroup);
                CreateDropItemGroup(0, "Dark Horse Spirit", 102);
                CreateDropItemGroup(1, "Dark Raven Spirit", 96);
            }

            void DeleteDropItemGroup(DropItemGroup group)
            {
                group.PossibleItems.Clear();
                foreach (var map in maps)
                {
                    if (map.DropItemGroups.FirstOrDefault(dig => dig.GetId() == group.GetId()) is { } mapDropItemGroup)
                    {
                        map.DropItemGroups.Remove(mapDropItemGroup);
                    }
                }

                gameConfiguration.DropItemGroups.Remove(group);
                context.DeleteAsync(group);
            }

            void CreateDropItemGroup(int itemLevel, string description, short minimumMonsterLevel)
            {
                var group = context.CreateNew<DropItemGroup>();
                group.SetGuid(NumberConversionExtensions.MakeWord(13, 31).ToSigned(), (short)itemLevel, 1);
                group.ItemLevel = (byte)itemLevel;
                group.Chance = 0.001;
                group.Description = description;
                group.PossibleItems.Add(spirit);
                group.MinimumMonsterLevel = (byte)minimumMonsterLevel;

                gameConfiguration.DropItemGroups.Add(group);
                foreach (var map in maps)
                {
                    map.DropItemGroups.Add(group);
                }
            }
        }

        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var goblinCraftings = gameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.ChaosMachine).ItemCraftings;
        var petTrainercraftings = gameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.PetTrainer).ItemCraftings;
        this.ApplyFirstWingsCraftingUpdate(goblinCraftings);
        this.ApplyDinorantCraftingUpdate(goblinCraftings);
        this.ApplyDinorantOptionsUpdate(gameConfiguration);

        // Fenrir dmg decrease fix
        Guid fenrirOptionsId = new("00000083-0081-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.Single(iod => iod.GetId() == fenrirOptionsId) is { } fenrirOpts
            && fenrirOpts.PossibleOptions.Single(iio => iio.PowerUpDefinition?.TargetAttribute == Stats.DamageReceiveDecrement) is { } dmgDecreaseOpt
            && dmgDecreaseOpt.PowerUpDefinition?.Boost is { } boost)
        {
            boost.ConstantValue.Value = 0.90f;
        }

        // Wizard's Ring wizardry option fix
        Guid wizardsRingId = new("00000080-000d-0014-0000-000000000000");
        if (gameConfiguration.Items.Single(id => id.GetId() == wizardsRingId) is { } wizardsRing)
        {
            wizardsRing.Durability = 30;

            if (wizardsRing.PossibleItemOptions.FirstOrDefault() is { } wizardsRingOpts)
            {
                wizardsRingOpts.MaximumOptionsPerItem = 3;

                var increaseWizardryDamage = context.CreateNew<IncreasableItemOption>();
                increaseWizardryDamage.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 3);
                increaseWizardryDamage.PowerUpDefinition = context.CreateNew<PowerUpDefinition>();
                increaseWizardryDamage.PowerUpDefinition.TargetAttribute = Stats.WizardryAttackDamageIncrease.GetPersistent(gameConfiguration);
                increaseWizardryDamage.PowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
                increaseWizardryDamage.PowerUpDefinition.Boost.ConstantValue.Value = 0.1f;
                wizardsRingOpts.PossibleOptions.Add(increaseWizardryDamage);
            }
        }

        // Remove Raven's lvl requirement
        Guid darkRavenId = new("00000080-000d-0005-0000-000000000000");
        if (gameConfiguration.Items.Single(id => id.GetId() == darkRavenId) is { } darkRaven)
        {
            darkRaven.Requirements.Clear();
        }

        // Aura/Storm Blitz Set definitions fix
        for (int i = 7; i <= 11; i++)
        {
            if (gameConfiguration.Items.FirstOrDefault(id => id.Group == i && id.Number == 43) is { } auraItem)
            {
                if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.GuardianOption && po.Number == i)) is { } guardOpt)
                {
                    auraItem.PossibleItemOptions.Add(guardOpt);
                }

                if (auraItem.Requirements.FirstOrDefault(r => r.Attribute == Stats.Level) is { } auraLvlRequirement)
                {
                    auraLvlRequirement.MinimumValue = 380;
                }
            }
        }

        // Phoenix Soul Set definitions fix
        for (int i = 7; i <= 11; i++)
        {
            if (gameConfiguration.Items.FirstOrDefault(id => id.Group == i && id.Number == 73) is { } phoenixSoulItem)
            {
                if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.GuardianOption && po.Number == i)) is { } guardOpt)
                {
                    phoenixSoulItem.PossibleItemOptions.Add(guardOpt);
                }

                if (phoenixSoulItem.Requirements.FirstOrDefault(r => r.Attribute == Stats.Level) is { } psLvlRequirement)
                {
                    psLvlRequirement.MinimumValue = 380;
                }
            }
        }

        // Phoenix Soul Star definition fix
        Guid phoenixSoulStarId = new("00000080-0000-0023-0000-000000000000");
        if (gameConfiguration.Items.FirstOrDefault(id => id.GetId() == phoenixSoulStarId) is { } phoenixSoulStar)
        {
            if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.GuardianOption && po.Number == (int)ItemGroups.Weapon)) is { } weapGuardOpt)
            {
                phoenixSoulStar.PossibleItemOptions.Add(weapGuardOpt);
            }

            if (phoenixSoulStar.Requirements.FirstOrDefault(r => r.Attribute == Stats.Level) is { } pssLvlRequirement)
            {
                pssLvlRequirement.MinimumValue = 380;
            }
        }

        // ---> Fix chaos mixes settings
        // Item Level Upgrade craftings
        int[] itemLevelUpgradeCraftingNos = [3, 4, 22, 23, 49, 50];
        for (int i = 0; i < itemLevelUpgradeCraftingNos.Length; i++)
        {
            if (goblinCraftings.Single(c => c.Number == itemLevelUpgradeCraftingNos[i])?.SimpleCraftingSettings is { } craftingSettings)
            {
                craftingSettings.Money = 2_000_000 * (10 + i - 9);
                craftingSettings.SuccessPercentageAdditionForLuck = 25;
                craftingSettings.SuccessPercentageAdditionForAncientItem = -10;
                craftingSettings.SuccessPercentageAdditionForGuardianItem = -10;

                foreach (var item in craftingSettings.RequiredItems)
                {
                    item.FailResult = MixResult.Disappear;
                    if (item.MaximumAmount == 0)
                    {
                        item.MaximumAmount = item.MinimumAmount;
                    }
                }
            }
        }

        // Second Wings crafting
        if (goblinCraftings.Single(c => c.Number == 7) is { } secondWingsCrafting)
        {
            secondWingsCrafting.ItemCraftingHandlerClassName = typeof(SecondWingsCrafting).FullName!;

            if (secondWingsCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.MoneyPerFinalSuccessPercentage = 0;

                foreach (var requiredItem in settings.RequiredItems)
                {
                    if (requiredItem.FailResult != MixResult.Disappear)
                    {
                        // 1st level wings or exc item
                        requiredItem.FailResult = MixResult.Disappear;
                    }
                    else
                    {
                        // chaos or feather
                        requiredItem.AddPercentage = 0;
                        requiredItem.MaximumAmount = 1;
                    }
                }

                settings.ResultItemLuckOptionChance = 20;
            }
        }

        // Thirds Wings, Stage 1 crafting
        if (goblinCraftings.Single(c => c.Number == 38) is { } thirdWingsS1Crafting)
        {
            if (thirdWingsS1Crafting.SimpleCraftingSettings is { } settings)
            {
                settings.SuccessPercent = 1;

                foreach (var requiredItem in settings.RequiredItems)
                {
                    if (requiredItem.FailResult != MixResult.Disappear)
                    {
                        // 2nd level wings or anc item
                        requiredItem.FailResult = MixResult.ThirdWingsDowngradedRandom;
                        if (requiredItem.MinimumItemLevel == 9)
                        {
                            // 2nd lvl wings
                            requiredItem.NpcPriceDivisor = 0;
                        }
                    }
                }
            }
        }

        // Thirds Wings, Stage 2 crafting
        if (goblinCraftings.Single(c => c.Number == 39) is { } thirdWingsS2Crafting)
        {
            thirdWingsS2Crafting.ItemCraftingHandlerClassName = typeof(ThirdWingsCrafting).FullName!;

            if (thirdWingsS2Crafting.SimpleCraftingSettings is { } settings)
            {
                settings.Money = 0;
                settings.MoneyPerFinalSuccessPercentage = 200_000;
                settings.SuccessPercent = 1;

                foreach (var requiredItem in settings.RequiredItems)
                {
                    if (requiredItem.FailResult != MixResult.Disappear)
                    {
                        // exc item
                        requiredItem.MinimumAmount = 1;
                        requiredItem.FailResult = MixResult.ThirdWingsDowngradedRandom;
                    }
                    else
                    {
                        requiredItem.MaximumAmount = 1;
                    }
                }

                settings.ResultItemLuckOptionChance = 5;
                settings.ResultItemExcellentOptionChance = 0;
                settings.ResultItemMaxExcOptionCount = 0;
            }
        }

        // Cape crafting
        if (goblinCraftings.Single(c => c.Number == 24) is { } capeCrafting)
        {
            capeCrafting.ItemCraftingHandlerClassName = typeof(SecondWingsCrafting).FullName!;

            if (capeCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.MoneyPerFinalSuccessPercentage = 0;

                foreach (var requiredItem in settings.RequiredItems)
                {
                    if (requiredItem.FailResult != MixResult.Disappear)
                    {
                        // 1st level wings or exc item
                        requiredItem.FailResult = MixResult.Disappear;

                        if (requiredItem.NpcPriceDivisor == 4_000_000)
                        {
                            // 1st level wings
                            requiredItem.MinimumItemLevel = 0;
                        }
                    }
                    else
                    {
                        // chaos or crest
                        requiredItem.AddPercentage = 0;
                        requiredItem.MaximumAmount = 1;
                    }
                }

                settings.ResultItemLuckOptionChance = 20;
                settings.ResultItemExcellentOptionChance = 20;
                settings.ResultItemMaxExcOptionCount = 1;
            }
        }

        // Fruit crafting
        if (goblinCraftings.Single(c => c.Number == 6) is { } fruitCrafting)
        {
            fruitCrafting.ItemCraftingHandlerClassName = typeof(SecondWingsCrafting).FullName!;

            if (fruitCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.MoneyPerFinalSuccessPercentage = 0;

                foreach (var resultItem in settings.ResultItems)
                {
                    resultItem.RandomMaximumLevel = 0;
                }
            }
        }

        // Small, Medium, and Large Shield Potion craftings
        for (int i = 30; i <= 32; i++)
        {
            if (goblinCraftings.Single(c => c.Number == i) is { } smallShieldPotCrafting)
            {
                if (smallShieldPotCrafting.SimpleCraftingSettings is { } settings)
                {
                    foreach (var resultItem in settings.ResultItems)
                    {
                        resultItem.Durability = 1;
                    }
                }
            }
        }

        // Guardian Option crafting
        if (goblinCraftings.Single(c => c.Number == 36) is { } guardianOptionCrafting)
        {
            if (guardianOptionCrafting.SimpleCraftingSettings is { } settings)
            {
                foreach (var requiredItem in settings.RequiredItems.OrderBy(i => i.MinimumItemLevel))
                {
                    if (requiredItem.MinimumItemLevel < 10)
                    {
                        continue;
                    }
                    else if (requiredItem.MinimumItemLevel == 10)
                    {
                        requiredItem.MaximumItemLevel = 15;
                    }
                    else
                    {
                        settings.RequiredItems.Remove(requiredItem);
                        break;
                    }
                }
            }
        }

        // Secromicon crafting
        if (goblinCraftings.Single(c => c.Number == 46) is { } secromiconCrafting)
        {
            if (secromiconCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.Money = 1_000_000;
            }
        }

        // Dark Horse crafting
        if (petTrainercraftings.Single(c => c.Number == 13) is { } darkHorseCrafting)
        {
            if (darkHorseCrafting.SimpleCraftingSettings is { } settings)
            {
                if (settings.ResultItems.First() is { } darkHorseResult)
                {
                    darkHorseResult.RandomMinimumLevel = 1;
                    darkHorseResult.RandomMaximumLevel = 1;
                }
            }
        }

        // Dark Raven crafting
        if (petTrainercraftings.Single(c => c.Number == 14) is { } darkRavenCrafting)
        {
            if (darkRavenCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.ResultItemSkillChance = 0;
                if (settings.ResultItems.First() is { } darkRavenResult)
                {
                    darkRavenResult.RandomMinimumLevel = 1;
                    darkRavenResult.RandomMaximumLevel = 1;
                }
            }
        }
    }
}