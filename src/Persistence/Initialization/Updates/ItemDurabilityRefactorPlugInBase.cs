// <copyright file="ItemDurabilityRefactorPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update resets some game configuration values which are used for item durability reduction.
/// </summary>
public abstract class ItemDurabilityRefactorPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Item Durability Refactor";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update resets some game configuration values which are used for item durability reduction.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 9, 22, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.JewelryAndWingsDurationIncrease);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.PetDurationIncrease);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.WeaponDurationIncrease);

        var jewelryAndWingsDurationIncrease = Stats.JewelryAndWingsDurationIncrease.GetPersistent(gameConfiguration);

        // Add new base attribute
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            void AddBaseAttributeIfNotExists(AttributeDefinition attribute)
            {
                if (charClass.BaseAttributeValues.All(ba => ba.Definition != attribute))
                {
                    charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1f, jewelryAndWingsDurationIncrease, AggregateType.AddRaw));
                }
            }

            AddBaseAttributeIfNotExists(jewelryAndWingsDurationIncrease);
        });

        gameConfiguration.DamagePerOneItemDurability = 69;
        gameConfiguration.DamagePerOnePetDurability = 100;
        gameConfiguration.HitsPerOneItemDurability = 564;
    }
}
