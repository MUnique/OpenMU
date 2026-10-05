// <copyright file="AddCastleSiegeDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Castle Siege configuration and persistent state to an existing Season 6 database.
/// </summary>
/// <remarks>
/// Version 2 additionally ensures the follow-up configuration which was added after
/// the initial update: the Sign of Lord registration item, the participant effects,
/// the Life Stone combat attributes and the Senior NPC economy window.
/// Every step only fills in missing or seeded values, so customized values
/// are preserved and re-running stays a no-op.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddCastleSiegeDataUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.AddCastleSiegeDataUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("CD201E33-37C9-4C85-95CC-16042B28E974")]
public class AddCastleSiegeDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Castle Siege data";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Castle Siege configuration and persistent state.";

    private const short SeniorNpcNumber = 223;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override int Version => 2;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 28, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 09, 30, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var initializer = new CastleSiegeInitializer(context, gameConfiguration);

        // Creates the configuration with all follow-up data if it's missing,
        // otherwise only ensures the registration, participant and Life Stone data.
        var configuration = initializer.InitializeConfiguration();
        if (!(await context.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Any())
        {
            initializer.InitializeData(configuration);
        }

        var senior = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == SeniorNpcNumber);
        if (senior is not null)
        {
            senior.NpcWindow = NpcWindow.CastleSeniorNPC;
        }
    }
}
