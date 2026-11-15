// <copyright file="AddHitShockSettingsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Adds the hit-shock settings and the <see cref="Stats.IsUniriaEquipped"/> attribute to the game configuration.
/// </summary>
public abstract class AddHitShockSettingsPlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.AddHitShockSettingsPlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddHitShockSettingsPlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 4, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.HitShockChance);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.HitShockDamageThreshold);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.IsUniriaEquipped);

        this.AddGlobalBaseAttributeValueIfNotExists(context, gameConfiguration, Stats.HitShockChance, 0.25f);
        this.AddGlobalBaseAttributeValueIfNotExists(context, gameConfiguration, Stats.HitShockDamageThreshold, 5f);

        var uniriaAttribute = Stats.IsUniriaEquipped.GetPersistent(gameConfiguration);

        foreach (var uniria in gameConfiguration.Items.Where(item => item.Group == 13 && item.Number == 2))
        {
            if (uniria.BasePowerUpAttributes.Any(powerUp => powerUp.TargetAttribute?.Id == uniriaAttribute.Id))
            {
                continue;
            }

            var powerUp = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUp.TargetAttribute = uniriaAttribute;
            powerUp.BaseValue = 1;
            powerUp.AggregateType = AggregateType.AddRaw;
            uniria.BasePowerUpAttributes.Add(powerUp);
        }

        return ValueTask.CompletedTask;
    }

    private void AddGlobalBaseAttributeValueIfNotExists(IContext context, GameConfiguration gameConfiguration, AttributeDefinition attribute, float value)
    {
        var persistentAttribute = attribute.GetPersistent(gameConfiguration);
        if (gameConfiguration.GlobalBaseAttributeValues.Any(existingValue => existingValue.Definition?.Id == persistentAttribute.Id))
        {
            return;
        }

        var globalValue = context.CreateNew<ConstValueAttribute>(value, persistentAttribute, AggregateType.AddRaw);
        gameConfiguration.GlobalBaseAttributeValues.Add(globalValue);
    }
}
