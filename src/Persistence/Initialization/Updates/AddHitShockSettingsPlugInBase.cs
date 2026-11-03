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
/// Adds the <see cref="Stats.IsUniriaEquipped"/> to the game configuration so Uniria-equipped players are excluded from hit shock.
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
    public override DateTime CreatedAt => new(2026, 11, 3, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.IsUniriaEquipped);
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
}
