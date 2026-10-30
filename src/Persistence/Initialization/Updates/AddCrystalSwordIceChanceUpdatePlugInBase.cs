// <copyright file="AddCrystalSwordIceChanceUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Adds the Crystal Sword ice chance, which turns an attack into an ice skill instead.
/// </summary>
public abstract class AddCrystalSwordIceChanceUpdatePlugInBase : UpdatePlugInBase
{
    private const short CrystalSwordNumber = 5;

    /// <inheritdoc />
    public override string Name => PlugInResources.AddCrystalSwordIceChanceUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.AddCrystalSwordIceChanceUpdatePlugIn_Description;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 23, 12, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.CrystalSwordIceChance);

        var iceChance = Stats.CrystalSwordIceChance.GetPersistent(gameConfiguration);
        var crystalSword = gameConfiguration.Items.FirstOrDefault(item => item.Group == (short)ItemGroups.Scepters && item.Number == CrystalSwordNumber);
        if (crystalSword is null)
        {
            return ValueTask.CompletedTask;
        }

        var powerUp = crystalSword.BasePowerUpAttributes.FirstOrDefault(bpua => bpua.TargetAttribute == iceChance);
        if (powerUp is null)
        {
            powerUp = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUp.TargetAttribute = iceChance;
            crystalSword.BasePowerUpAttributes.Add(powerUp);
        }

        powerUp.BaseValue = 1.0f / 20;
        powerUp.AggregateType = AggregateType.AddRaw;

        return ValueTask.CompletedTask;
    }
}
