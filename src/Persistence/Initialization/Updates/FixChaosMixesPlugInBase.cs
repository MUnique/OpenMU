// <copyright file="FixChaosMixesPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

/// <summary>
/// This update fixes the Chaos Weapon crafting settings.
/// </summary>
public abstract class FixChaosMixesPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Chaos Mixes Settings";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes Chaos Weapon crafting settings.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 12, 10, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Misc item fixes
        Guid crystalSwordId = new("00000080-0002-0005-0000-000000000000");
        if (gameConfiguration.Items.FirstOrDefault(id => id.GetId() == crystalSwordId) is { } crystalSword)
        {
            crystalSword.Width = 2;
        }

        Guid powerWaveScrollId = new("00000080-000f-000a-0000-000000000000");
        if (gameConfiguration.Items.FirstOrDefault(id => id.GetId() == powerWaveScrollId) is { } powerWaveScroll)
        {
            powerWaveScroll.Value = 1100;
        }

        // Fix Chaos Weapon crafting
        var craftings = gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine).ItemCraftings;

        if (craftings.Single(c => c.Number == 1) is { } chaosWeaponCrafting)
        {
            chaosWeaponCrafting.ItemCraftingHandlerClassName = typeof(ChaosWeaponAndFirstWingsCrafting).FullName!;

            if (chaosWeaponCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.SuccessPercent = 0;
                settings.NpcPriceDivisor = 20_000;

                foreach (var requiredItem in settings.RequiredItems)
                {
                    requiredItem.AddPercentage = 0;
                    if (requiredItem.FailResult != MixResult.Disappear)
                    {
                        requiredItem.NpcPriceDivisor = 0;
                        requiredItem.FailResult = MixResult.ChaosWeaponAndFirstWingsDowngradedRandom;
                    }
                }

                settings.ResultItemLuckOptionChance = 0;
                settings.ResultItemSkillChance = 0;

                foreach (var resultItem in settings.ResultItems)
                {
                    resultItem.RandomMinimumLevel = 0;
                    resultItem.RandomMaximumLevel = 4;
                }
            }
        }
    }

    /// <summary>
    /// Applies First Wings crafting settings update.
    /// </summary>
    /// <param name="craftings">The craftings collection.</param>
    protected void ApplyFirstWingsCraftingUpdate(ICollection<ItemCrafting> craftings)
    {
        if (craftings.Single(c => c.Number == 11) is { } firstWingsCrafting)
        {
            firstWingsCrafting.ItemCraftingHandlerClassName = typeof(ChaosWeaponAndFirstWingsCrafting).FullName!;

            if (firstWingsCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.SuccessPercent = 0;
                settings.NpcPriceDivisor = 20_000;

                foreach (var requiredItem in settings.RequiredItems)
                {
                    requiredItem.AddPercentage = 0;
                    requiredItem.NpcPriceDivisor = 0;
                    if (requiredItem.MaximumAmount == 1)
                    {
                        // Chaos weapon
                        requiredItem.FailResult = MixResult.ChaosWeaponAndFirstWingsDowngradedRandom;
                    }
                    else
                    {
                        requiredItem.FailResult = MixResult.Disappear;
                    }
                }

                settings.ResultItemLuckOptionChance = 0;
            }
        }
    }

    /// <summary>
    /// Applies Dinorant crafting settings update.
    /// </summary>
    /// <param name="craftings">The craftings collection.</param>
    protected void ApplyDinorantCraftingUpdate(ICollection<ItemCrafting> craftings)
    {
        if (craftings.Single(c => c.Number == 5) is { } dinorantCrafting)
        {
            dinorantCrafting.ItemCraftingHandlerClassName = typeof(DinorantCrafting).FullName!;

            if (dinorantCrafting.SimpleCraftingSettings is { } settings)
            {
                settings.ResultItemExcellentOptionChance = 0;
            }
        }
    }

    /// <summary>
    /// Applies Dinorant options update.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected void ApplyDinorantOptionsUpdate(GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.ItemOptions.Single(iod => iod.Name == "Dinorant Options") is { } dinoOpts
            && gameConfiguration.ItemOptionTypes.Single(iot => iot == ItemOptionTypes.Option) is { } itemOption)
        {
            dinoOpts.AddChance = 0.3f;

            foreach (var opt in dinoOpts.PossibleOptions)
            {
                opt.OptionType = itemOption;
                opt.Number = 4;
            }
        }
    }
}