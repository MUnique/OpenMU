// <copyright file="AddGlobalMoneyAmountRateAttributePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update moves the <see cref="Stats.MoneyAmountRate"/> attribute from individual character class
/// base attribute values to the global base attribute values of the game configuration.
/// </summary>
public abstract class AddGlobalMoneyAmountRateAttributePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add Global Money Amount Rate Attribute";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Moves the MoneyAmountRate attribute from character class base attributes to global base attributes.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 03, 20, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var moneyAmountRateDef = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == Stats.MoneyAmountRate.Id);
        if (moneyAmountRateDef is null)
        {
            return;
        }

        if (!gameConfiguration.GlobalBaseAttributeValues.Any(a => a.Definition?.Id == Stats.MoneyAmountRate.Id))
        {
            var globalMoneyRate = context.CreateNew<ConstValueAttribute>(1f, moneyAmountRateDef);
            gameConfiguration.GlobalBaseAttributeValues.Add(globalMoneyRate);
        }

        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            var classMoneyRate = characterClass.BaseAttributeValues
                .FirstOrDefault(a => a.Definition?.Id == Stats.MoneyAmountRate.Id);
            if (classMoneyRate is not null)
            {
                characterClass.BaseAttributeValues.Remove(classMoneyRate);
            }
        }
    }
}
