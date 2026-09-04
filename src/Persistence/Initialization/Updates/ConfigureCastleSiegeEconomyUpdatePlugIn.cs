// <copyright file="ConfigureCastleSiegeEconomyUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures the Castle Siege economy interface for an existing Season 6 database.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("7ED67868-5C82-4B10-9BDA-732F51704DB9")]
public class ConfigureCastleSiegeEconomyUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Configure Castle Siege economy";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update enables the Castle Siege economy interface of the Senior NPC.";

    private const short SeniorNpcNumber = 223;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureCastleSiegeEconomy;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 08, 27, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var senior = gameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == SeniorNpcNumber);
        if (senior is not null)
        {
            senior.NpcWindow = NpcWindow.CastleSeniorNPC;
        }

        return ValueTask.CompletedTask;
    }
}
