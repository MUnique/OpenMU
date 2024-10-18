// <copyright file="FixWingsDmgRatesUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update fixes the wings damage absorption and increase bonus level tables values for a <see cref="CombinedElement"/> (sum) calculation, instead of a compound calculation.
/// </summary>
public abstract class FixWingsDmgRatesUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Wings Damage Rates";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the wings damage absorption and increase bonus level tables values.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 10, 8, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        string dmgAbsorbCommonName = "Wing absorb";
        string dmgIncreaseCommonName = "Damage Increase (1st and 3rd Wings)";
        string dmgIncrease2ndWingsName = "Damage Increase (2nd Wings)";

        string[] wingDmgTableNames = [dmgAbsorbCommonName, dmgIncrease2ndWingsName, dmgIncreaseCommonName];

        foreach (var tableName in wingDmgTableNames)
        {
            var bonusEntries = gameConfiguration.ItemLevelBonusTables.FirstOrDefault(ilbt => ilbt.Name == tableName)?.BonusPerLevel;

            if (bonusEntries is not null)
            {
                foreach (var bonusEntry in bonusEntries)
                {
                    bonusEntry.AdditionalValue -= 1.0f;
                }
            }
        }
    }
}