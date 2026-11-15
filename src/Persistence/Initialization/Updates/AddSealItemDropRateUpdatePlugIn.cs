// <copyright file="AddSealItemDropRateUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the higher item drop rate to the effects of the Seals of Wealth and Divinity
/// and the Master Seal of Wealth, and adds the second Seal of Wealth (13/116), which only raises
/// the item drop rate.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSealItemDropRateUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddSealItemDropRateUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("A2377A4A-23E9-40DE-BEA0-6BB457E14E52")]
public class AddSealItemDropRateUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add the item drop rate of seals";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the item drop rate of 150 % to the Seals of Wealth and Divinity and the Master Seal of Wealth, and adds the second Seal of Wealth (13/116), which only raises the item drop rate.";

    private static readonly MagicEffectNumber[] SealEffects =
    [
        MagicEffectNumber.SealWealth,
        MagicEffectNumber.SealOfDivinity,
        MagicEffectNumber.MasterSealOfWealth,
    ];

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 09, 6, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    /// <remarks>
    /// The seals and their effects are added by the <see cref="AddBuffItemsUpdatePlugIn"/>.
    /// </remarks>
    public override IEnumerable<UpdateDependency> DependsOn => [typeof(AddBuffItemsUpdatePlugIn).GUID];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var dropRate = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == Stats.BonusItemDropRate.Id);
        if (dropRate is null)
        {
            dropRate = context.CreateNew<AttributeDefinition>(Stats.BonusItemDropRate.Id, Stats.BonusItemDropRate.Designation, Stats.BonusItemDropRate.Description);
            gameConfiguration.Attributes.Add(dropRate);
        }

        foreach (var effectNumber in SealEffects)
        {
            if (gameConfiguration.MagicEffects.FirstOrDefault(e => e.Number == (short)effectNumber) is not { } effect
                || effect.PowerUpDefinitions.Any(p => p.TargetAttribute?.Id == dropRate.Id))
            {
                continue;
            }

            var powerUp = context.CreateNew<PowerUpDefinition>();
            powerUp.TargetAttribute = dropRate;
            powerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUp.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
            powerUp.Boost.ConstantValue.Value = BuffItems.SealItemDropRateBonus;
            effect.PowerUpDefinitions.Add(powerUp);
        }

        var addedItems = new BuffItems(context, gameConfiguration).CreateItems();
        ItemRules.Apply(addedItems);
        return ValueTask.CompletedTask;
    }
}
