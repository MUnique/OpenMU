// <copyright file="AddHarmonyOptionWeightsUpdateSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds Jewel of Harmony option weights used for option assignment, fixes some options, and fixes item restore mix.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("E94DE59E-5B3A-4498-A4AF-E7F4F173B754")]
public class AddHarmonyOptionWeightsUpdateSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add harmony option weights";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds Jewel of Harmony option weights used for option assignment, fixes some options, and fixes item restore mix";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddHarmonyOptionWeightsSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 10, 24, 15, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Fix Restore Item Mix
        var requiredItem = gameConfiguration.Monsters
            .Single(m => m.NpcWindow == NpcWindow.RemoveJohOption).ItemCraftings
            .Single(ic => ic.Number == 35).SimpleCraftingSettings?.RequiredItems
            .Single();
        if (requiredItem is { } item)
        {
            item.MaximumAmount = 1;
            item.MaximumItemLevel = 15;
        }

        // Add JoH option weights
        byte[] defOptWeights = [50, 40, 40, 30, 20, 20, 20, 10];
        byte[] physAttackOptWeights = [40, 40, 40, 40, 30, 30, 20, 20, 20, 10];
        byte[] magicOptWeights = [40, 40, 40, 30, 30, 20, 20, 10];

        var defOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.DefenseOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);
        var physAttackOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.PhysicalAttackOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);
        var wizAttackOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.WizardryAttackOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);
        var curseAttackOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.CurseAttackOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);

        if (defOptions?.Count() == defOptWeights.Length)
        {
            for (int i = 0; i < defOptWeights.Length; i++)
            {
                defOptions.ElementAt(i).Weight = defOptWeights[i];
            }
        }

        if (physAttackOptions?.Count() == physAttackOptWeights.Length)
        {
            for (int i = 0; i < physAttackOptWeights.Length; i++)
            {
                physAttackOptions.ElementAt(i).Weight = physAttackOptWeights[i];
            }
        }

        if (wizAttackOptions?.Count() == magicOptWeights.Length)
        {
            for (int i = 0; i < magicOptWeights.Length; i++)
            {
                wizAttackOptions.ElementAt(i).Weight = magicOptWeights[i];
            }
        }

        if (curseAttackOptions?.Count() == magicOptWeights.Length)
        {
            for (int i = 0; i < magicOptWeights.Length; i++)
            {
                curseAttackOptions.ElementAt(i).Weight = magicOptWeights[i];
            }
        }

        // Fix physical base dmg attribute
        var baseDmgBonusOpt = physAttackOptions?.Single(o => o.Number == 5);
        var physBaseDmgAttr = gameConfiguration.Attributes.Single(a => a.Id == new Guid("DD1E13E4-BFFD-45B5-9B91-9080710324B2"));

        if (baseDmgBonusOpt?.LevelDependentOptions is ICollection<ItemOptionOfLevel> baseDmgOptLvls)
        {
            foreach (var level in baseDmgOptLvls)
            {
                if (level.PowerUpDefinition is PowerUpDefinition pud)
                {
                    pud.TargetAttribute = physBaseDmgAttr;
                }
            }
        }

        // Fix wiz/curse dmg increase option values and attribute
        var wizAtkDmgIncOpt = wizAttackOptions?.Single(o => o.Number == 1);
        var curseAtkDmgIncOpt = curseAttackOptions?.Single(o => o.Number == 1);
        List<IncreasableItemOption?> magicAtkDmgIncOpts = [wizAtkDmgIncOpt, curseAtkDmgIncOpt];

        var wizBaseDmgAttr = gameConfiguration.Attributes.Single(a => a.Id == new Guid("7F4F3646-33A6-40AC-8DA6-29A0A0F46016"));
        float[] magicAtkDmgIncValues = [6, 8, 10, 12, 14, 16, 17, 18, 19, 21, 23, 25, 27, 31];

        foreach (var opt in magicAtkDmgIncOpts)
        {
            var optLvls = opt?.LevelDependentOptions.OrderBy(ldo => ldo.Level);
            if (optLvls?.Count() == magicAtkDmgIncValues.Length)
            {
                for (int i = 0; i < magicAtkDmgIncValues.Length; i++)
                {
                    if (optLvls.ElementAt(i).PowerUpDefinition is PowerUpDefinition pud)
                    {
                        pud.TargetAttribute = wizBaseDmgAttr;

                        if (pud.Boost?.ConstantValue is SimpleElement element)
                        {
                            element.Value = magicAtkDmgIncValues[i];
                        }
                    }
                }
            }
        }
    }
}