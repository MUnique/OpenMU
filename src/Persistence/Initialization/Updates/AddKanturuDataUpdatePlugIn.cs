// <copyright file="AddKanturuDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Kanturu Refinery Tower event configuration to an existing Season 6 database.
/// </summary>
/// <remarks>
/// The <see cref="KanturuInitializer"/> only runs when a database is created from scratch,
/// so databases which were initialized before the Kanturu event existed are missing its
/// <see cref="MiniGameDefinition"/>. This update adds it without touching any other data.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("3F1B7A64-9C2E-4D58-B0A7-5E6C8D19F204")]
public class AddKanturuDataUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Kanturu data";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the Kanturu Refinery Tower event configuration.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddKanturuData;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 08, 30, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.MiniGameDefinitions.Any(d => d.Type == MiniGameType.Kanturu))
        {
            // Already added, nothing to do.
            return ValueTask.CompletedTask;
        }

        new KanturuInitializer(context, gameConfiguration).Initialize();
        return ValueTask.CompletedTask;
    }
}
