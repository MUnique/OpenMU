// <copyright file="ItemDurabilityRefactorPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update resets some game configuration values which are used for item durability reduction. It also fixes the master skill tree durability reduction skills.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("D8A4F1C7-6B2E-49D3-A5F0-1C7E9B4D2A86")]
public class ItemDurabilityRefactorPlugInSeason6 : ItemDurabilityRefactorPlugInBase
{
    /// <summary>
    /// The plug in description.
    /// </summary>
    private new const string PlugInDescription = "This update resets some game configuration values which are used for item durability decrement. It also fixes the master skill tree durability reduction skills.";

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ItemDurabilityRefactorSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var jewelryAndWingsDurationIncrease = Stats.JewelryAndWingsDurationIncrease.GetPersistent(gameConfiguration);
        var petDurationIncrease = Stats.PetDurationIncrease.GetPersistent(gameConfiguration);
        var weaponDurationIncrease = Stats.WeaponDurationIncrease.GetPersistent(gameConfiguration);
        var trainablePetDurationIncrease = Stats.TrainablePetDurationIncrease.GetPersistent(gameConfiguration);

        // Remove TrainablePetDurationIncrease (old PetDurationIncrease)
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            if (charClass.Number == (byte)CharacterClasses.CharacterClassNumber.DarkLord
                || charClass.Number == (byte)CharacterClasses.CharacterClassNumber.LordEmperor)
            {
                if (charClass.BaseAttributeValues.FirstOrDefault(bav => bav.Definition == trainablePetDurationIncrease) is { } baseTrainablePetDurationIncrease)
                {
                    charClass.BaseAttributeValues.Remove(baseTrainablePetDurationIncrease);
                }
            }
        });

        // Update ice socket option
        var iceSocketOptionsId = new Guid("00000083-0034-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(io => io.GetId() == iceSocketOptionsId) is { } iceSocketOptions
            && iceSocketOptions.PossibleOptions.FirstOrDefault(po => po.Number == 4) is { } durationIncrease)
        {
            foreach (var levelOption in durationIncrease.LevelDependentOptions)
            {
                levelOption.PowerUpDefinition?.TargetAttribute = weaponDurationIncrease;
                levelOption.PowerUpDefinition?.Boost?.ConstantValue.Value -= 1.0f;
                levelOption.PowerUpDefinition?.Boost?.ConstantValue.AggregateType = AggregateType.AddRaw;
            }
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DurabilityReduction1)?.MasterDefinition is { } durabilityReduction1)
        {
            durabilityReduction1.ValueFormula = $"{SkillsInitializer.Formula1204} / 100";
            durabilityReduction1.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DurabilityReduction2)?.MasterDefinition is { } durabilityReduction2)
        {
            durabilityReduction2.ValueFormula = $"{SkillsInitializer.Formula1204} / 100";
            durabilityReduction2.Aggregation = AggregateType.AddRaw;
            durabilityReduction2.TargetAttribute = jewelryAndWingsDurationIncrease;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DurabilityReduction3)?.MasterDefinition is { } durabilityReduction3)
        {
            durabilityReduction3.Aggregation = AggregateType.AddRaw;
            durabilityReduction3.TargetAttribute = petDurationIncrease;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PetDurabilityStr)?.MasterDefinition is { } petDurabilityStr)
        {
            petDurabilityStr.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DurabilityReduction1FistMaster)?.MasterDefinition is { } durabilityReduction1FistMaster)
        {
            durabilityReduction1FistMaster.ValueFormula = $"{SkillsInitializer.Formula1204} / 100";
            durabilityReduction1FistMaster.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DurabilityReduction2FistMaster)?.MasterDefinition is { } durabilityReduction2FistMaster)
        {
            durabilityReduction2FistMaster.ValueFormula = $"{SkillsInitializer.Formula1204} / 100";
            durabilityReduction2FistMaster.Aggregation = AggregateType.AddRaw;
            durabilityReduction2FistMaster.TargetAttribute = jewelryAndWingsDurationIncrease;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DurabilityReduction3FistMaster)?.MasterDefinition is { } durabilityReduction3FistMaster)
        {
            durabilityReduction3FistMaster.Aggregation = AggregateType.AddRaw;
            durabilityReduction3FistMaster.TargetAttribute = petDurationIncrease;
        }
    }
}
