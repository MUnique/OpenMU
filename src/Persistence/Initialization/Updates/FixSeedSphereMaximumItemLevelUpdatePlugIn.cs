// <copyright file="FixSeedSphereMaximumItemLevelUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update sets the maximum item level of the seed spheres to the highest option number of their element.
/// The item level of a seed sphere defines its kind of option, so a maximum item level of 0
/// prevented creating seed spheres with any other option, e.g. with the item chat command.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixSeedSphereMaximumItemLevelUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixSeedSphereMaximumItemLevelUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("6A7C74F3-FD49-4013-BDC3-103B3D9E65E4")]
public class FixSeedSphereMaximumItemLevelUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The item group of the seed spheres.
    /// </summary>
    private const byte SeedSphereGroup = 12;

    /// <summary>
    /// The number of the first seed sphere item definition, as defined by the client.
    /// </summary>
    private const short SeedSphereNumberStart = 100;

    /// <summary>
    /// The number of the last seed sphere item definition, as defined by the client.
    /// </summary>
    private const short SeedSphereNumberEnd = 129;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => Properties.PlugInResources.FixSeedSphereMaximumItemLevelUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => Properties.PlugInResources.FixSeedSphereMaximumItemLevelUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 25, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var seedSpheres = gameConfiguration.Items.Where(item => item.Group == SeedSphereGroup
                                                                && item.Number >= SeedSphereNumberStart
                                                                && item.Number <= SeedSphereNumberEnd);
        foreach (var seedSphere in seedSpheres)
        {
            var maximumOptionNumber = seedSphere.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.SocketOption)
                .Select(o => o.Number)
                .DefaultIfEmpty()
                .Max();

            // Only the initial value is corrected, so a deliberately changed configuration stays as it is.
            if (seedSphere.MaximumItemLevel == 0)
            {
                seedSphere.MaximumItemLevel = (byte)maximumOptionNumber;
            }
        }

        return ValueTask.CompletedTask;
    }
}
