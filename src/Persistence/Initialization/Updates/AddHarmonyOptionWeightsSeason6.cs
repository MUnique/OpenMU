// <copyright file="AddHarmonyOptionWeightsSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds Jewel of Harmony option weights used for option assignment.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("E94DE59E-5B3A-4498-A4AF-E7F4F173B754")]
public class AddHarmonyOptionWeightsSeason6 : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add harmony option weights";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds Jewel of Harmony option weights used for option assignment";

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
    public override DateTime CreatedAt => new(2024, 10, 22, 18, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        byte[] defOptWeights = [50, 40, 40, 30, 20, 20, 20, 10];
        byte[] physAttackOptWeights = [40, 40, 40, 40, 30, 30, 20, 20, 20, 10];
        byte[] magicOptWeights = [40, 40, 40, 30, 30, 20, 20, 10];

        var defOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.DefenseOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);
        var physAttackOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.PhysicalAttackOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);
        var wizAttackOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.WizardryAttackOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);
        var curseAttackOptions = gameConfiguration.ItemOptions.Where(io => io.Name == HarmonyOptions.CurseAttackOptionsName)
            .FirstOrDefault()?.PossibleOptions.OrderBy(o => o.Number);

        if (defOptions?.Count() == defOptWeights.Length)
        {
            for (int i = 0; i < defOptWeights.Length; i++)
            {
                defOptions.ElementAt(i).Weight = defOptWeights[i];
            }
        }

        if (physAttackOptions?.Count() == physAttackOptWeights.Length)
        {
            for (int i = 0; i < physAttackOptWeights.Length; i++)
            {
                physAttackOptions.ElementAt(i).Weight = physAttackOptWeights[i];
            }
        }

        if (wizAttackOptions?.Count() == magicOptWeights.Length)
        {
            for (int i = 0; i < magicOptWeights.Length; i++)
            {
                wizAttackOptions.ElementAt(i).Weight = magicOptWeights[i];
            }
        }

        if (curseAttackOptions?.Count() == magicOptWeights.Length)
        {
            for (int i = 0; i < magicOptWeights.Length; i++)
            {
                curseAttackOptions.ElementAt(i).Weight = magicOptWeights[i];
            }
        }
    }
}