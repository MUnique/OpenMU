// <copyright file="AddElfSoldierBuffPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the Elf Soldier buff to the existing Elf Soldier NPC.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("9BCFC8B1-6A6E-48F9-AE7C-0D34FA6D706B")]
public class AddElfSoldierBuffPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Elf Soldier Buff";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Elf Soldier buff (defense and damage boost) as a configurable Buff.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddElfSoldierBuff;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 11, 17, 10, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var elfSoldier = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == 257);
        if (elfSoldier is null)
        {
            return;
        }

        if (elfSoldier.Buffs.Any())
        {
            return;
        }

        var buffEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(buffEffect);
        buffEffect.Number = (short)MagicEffectNumber.ElfSoldierBuff;
        buffEffect.Name = "Elf Soldier Buff";
        buffEffect.InformObservers = true;
        buffEffect.StopByDeath = true;

        // Duration: 60 minutes
        buffEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        buffEffect.Duration.ConstantValue.Value = 3600;

        // Defense boost: 50 + (Level / 5)
        var defensePowerUp = context.CreateNew<PowerUpDefinition>();
        defensePowerUp.TargetAttribute = Stats.DefenseFinal.GetPersistent(gameConfiguration);
        defensePowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        defensePowerUp.Boost.ConstantValue.Value = 50;
        defensePowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
        var defensePerLevel = context.CreateNew<AttributeRelationship>();
        defensePerLevel.InputAttribute = Stats.Level.GetPersistent(gameConfiguration);
        defensePerLevel.InputOperand = 1f / 5;
        defensePerLevel.InputOperator = InputOperator.Multiply;
        defensePowerUp.Boost.RelatedValues.Add(defensePerLevel);
        buffEffect.PowerUpDefinitions.Add(defensePowerUp);

        // Damage boost: 45 + (Level / 3)
        var damagePowerUp = context.CreateNew<PowerUpDefinition>();
        damagePowerUp.TargetAttribute = Stats.GreaterDamageBonus.GetPersistent(gameConfiguration);
        damagePowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        damagePowerUp.Boost.ConstantValue.Value = 45;
        var damagePerLevel = context.CreateNew<AttributeRelationship>();
        damagePerLevel.InputAttribute = Stats.Level.GetPersistent(gameConfiguration);
        damagePerLevel.InputOperand = 1f / 3;
        damagePerLevel.InputOperator = InputOperator.Multiply;
        damagePowerUp.Boost.RelatedValues.Add(damagePerLevel);
        buffEffect.PowerUpDefinitions.Add(damagePowerUp);

        var buff = context.CreateNew<Buff>();
        buff.MagicEffectDefinition = buffEffect;
        buff.MaximumLevel = 220;
        elfSoldier.Buffs.Add(buff);
    }
}
