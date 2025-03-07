// <copyright file="OptionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions to get <see cref="ItemOptionDefinition"/>s of the <see cref="GameConfiguration"/>.
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Gets the luck item option definition.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The definition for the luck option.</returns>
    public static ItemOptionDefinition GetLuck(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck));
    }

    /// <summary>
    /// Gets the defense option definition.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The definition for the defense option.</returns>
    public static ItemOptionDefinition GetDefenseOption(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.DefenseBase));
    }

    /// <summary>
    /// Gets the physical damage option definition.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The definition for the physical damage option.</returns>
    public static ItemOptionDefinition PhysicalDamageOption(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmg));
    }

    /// <summary>
    /// Gets the wizardry damage option definition.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The definition for the wizardry damage option.</returns>
    public static ItemOptionDefinition WizardryDamageOption(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.WizardryBaseDmg));
    }

    /// <summary>
    /// Gets the defense rate option definition.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The definition for the defense rate option.</returns>
    public static ItemOptionDefinition GetDefenseRateOption(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.DefenseRatePvm));
    }

    /// <summary>
    /// Gets the physical and wizardry damage option definition.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The definition for the physical and wizardry damage option.</returns>
    public static ItemOptionDefinition PhysicalAndWizardryDamageOption(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.BaseDamageBonus));
    }

    /// <summary>
    /// Gets the excellent defense options.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The excellent defense options.</returns>
    public static ItemOptionDefinition ExcellentDefenseOptions(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(o => o.Name == ExcellentOptions.DefenseOptionsName);
    }

    /// <summary>
    /// Gets the excellent physical attack options.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The excellent physical attack options.</returns>
    public static ItemOptionDefinition ExcellentPhysicalAttackOptions(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(o => o.Name == ExcellentOptions.PhysicalAttackOptionsName);
    }

    /// <summary>
    /// Gets the excellent wizardry attack options.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The excellent wizardry attack options.</returns>
    public static ItemOptionDefinition ExcellentWizardryAttackOptions(this GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.First(o => o.Name == ExcellentOptions.WizardryAttackOptionsName);
    }
}