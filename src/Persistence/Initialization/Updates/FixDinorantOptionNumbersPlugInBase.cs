// <copyright file="FixDinorantOptionNumbersPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update gives each Dinorant option a distinct number, which is its bit in the option field the client expects.
/// </summary>
public abstract class FixDinorantOptionNumbersPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Dinorant option numbers";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update gives each Dinorant option a distinct number, which is its bit in the option field the client expects.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 2, 23, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var numbersByAttribute = new Dictionary<AttributeDefinition, int>
        {
            { Stats.DamageReceiveDecrement, 1 },
            { Stats.MaximumAbility, 2 },
            { Stats.AttackSpeedAny, 4 },
        };

        if (gameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == "Dinorant Options") is not { } dinoOptions)
        {
            return ValueTask.CompletedTask;
        }

        foreach (var option in dinoOptions.PossibleOptions)
        {
            if (option.PowerUpDefinition?.TargetAttribute is { } targetAttribute
                && numbersByAttribute.TryGetValue(targetAttribute, out var number))
            {
                option.Number = number;
            }
        }

        return ValueTask.CompletedTask;
    }
}
