// <copyright file="FixJeweleryPetsDamageCalcsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// This update fixes jewelery items resistance values.
/// </summary>
public abstract class FixJeweleryPetsDamageCalcsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Jewelery Resistance Calculations";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes jewelery items resistance values.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 09, 30, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Add new attributes
        var bonusExperienceRate = context.CreateNew<AttributeDefinition>(Stats.BonusExperienceRate.Id, Stats.BonusExperienceRate.Designation, Stats.BonusExperienceRate.Description);
        gameConfiguration.Attributes.Add(bonusExperienceRate);
        var isPetSkeletionEquipped = context.CreateNew<AttributeDefinition>(Stats.IsPetSkeletonEquipped.Id, Stats.IsPetSkeletonEquipped.Designation, Stats.IsPetSkeletonEquipped.Description);
        gameConfiguration.Attributes.Add(isPetSkeletionEquipped);

        var resistancesTable = gameConfiguration.ItemLevelBonusTables.First(t => t.Name == "Elemental resistances (Jewelery)");
        resistancesTable.Description = "Defines the elemental resistances for jewelery. It's 1 per item level.";

        float[] resistanceIncreaseByLevel = [0, 1, 2, 3, 4];
        foreach (var levelBonus in resistancesTable.BonusPerLevel)
        {
            levelBonus.AdditionalValue = resistanceIncreaseByLevel[levelBonus.Level];
        }

        // Update jewelery
        var jeweleryItemSlots = gameConfiguration.ItemSlotTypes.Where(st => st.ItemSlots.Contains(9) || st.ItemSlots.Contains(10));
        var jewelery = gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Misc1 && jeweleryItemSlots.Contains(i.ItemSlot));

        var xfmRing = jewelery.First(i => i.Number == 10); // Transformation Ring
        this.AddLevelRequirement(context, gameConfiguration, xfmRing, 20);

        foreach (var jeweleryItem in jewelery)
        {
            if (jeweleryItem.BasePowerUpAttributes.FirstOrDefault(p =>
                    p.TargetAttribute == Stats.IceResistance ||
                    p.TargetAttribute == Stats.PoisonResistance ||
                    p.TargetAttribute == Stats.LightningResistance ||
                    p.TargetAttribute == Stats.FireResistance ||
                    p.TargetAttribute == Stats.EarthResistance ||
                    p.TargetAttribute == Stats.WindResistance ||
                    p.TargetAttribute == Stats.WaterResistance) is { } resistancePowerUp)
            {
                resistancePowerUp.BaseValue = 1;
            }
        }
    }

#pragma warning disable SA1600, CS1591
    protected void AddLevelRequirement(IContext context, GameConfiguration gameConfiguration, ItemDefinition item, int level)
    {
        var requirement = context.CreateNew<AttributeRequirement>();
        requirement.Attribute = Stats.Level.GetPersistent(gameConfiguration);
        requirement.MinimumValue = level;
        item.Requirements.Add(requirement);
    }
#pragma warning restore SA1600, CS1591
}