// <copyright file="FixMaxManaAndAbilityJewelryOptionsUpdateSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the calculation for max mana/AG % increase provided by Ring of Magic/Pendant of Ability options.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("EAC7C809-D4B8-443F-BE52-E56560003483")]
public class FixMaxManaAndAbilityJewelryOptionsUpdateSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix max mana/AG % increase jewelry options";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the calculation for max mana/AG % increase provided by Ring of Magic/Pendant of Ability options";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixMaxManaAndAbilityJewelryOptionsSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 09, 26, 10, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var itemOptionGuids = new List<Guid> { new("00000083-d018-8826-0000-000000000000"), new("00000083-d01c-bbba-0000-000000000000") };
        var itemOptions = gameConfiguration.ItemOptions.Where(io => itemOptionGuids.Contains(io.GetId()));

        foreach (var itemOption in itemOptions)
        {
            var optionsOfLevel = itemOption?.PossibleOptions.FirstOrDefault()?.LevelDependentOptions
                .Where(opt => opt.Level > 1)
                .OrderBy(opt => opt.Level);

            if (optionsOfLevel is not null)
            {
                for (int i = 0; i < optionsOfLevel.Count(); i++)
                {
                    if (optionsOfLevel.ElementAt(i).PowerUpDefinition?.Boost?.ConstantValue is SimpleElement elmt && elmt.Value > i + 2)
                    {
                        elmt.Value -= i + 1;
                    }
                }
            }
        }
    }
}