// <copyright file="VulcanusExperiencePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Increases the experience of a player in Vulcanus by its <see cref="Stats.VulcanusExperienceRate"/>,
/// which is given e.g. by the Gladiator's Honor.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.VulcanusExperiencePlugIn_Name), Description = nameof(PlugInResources.VulcanusExperiencePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("03E6A271-821E-49B8-B6E9-B24E95B6D16E")]
public class VulcanusExperiencePlugIn : IExperienceCalculationPlugIn, ISupportCustomConfiguration<VulcanusMapConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public VulcanusMapConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public ValueTask CalculateExperienceAsync(Player player, ExperienceCalculationArgs args)
    {
        var mapNumber = (this.Configuration ??= new VulcanusMapConfiguration()).MapNumber;
        if (player.CurrentMap?.MapId == mapNumber
            && player.Attributes?[Stats.VulcanusExperienceRate] is > 0 and var rate)
        {
            args.Experience *= 1 + rate;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new VulcanusMapConfiguration();
}
