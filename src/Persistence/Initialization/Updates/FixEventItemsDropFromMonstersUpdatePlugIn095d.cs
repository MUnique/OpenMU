// <copyright file="FixEventItemsDropFromMonstersUpdatePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes event items that have DropsFromMonsters set to true,
/// but should use dedicated DropItemGroups instead.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("B9C6D3E2-4F5A-6B7C-8D9E-0F1A2B3C4D5E")]
public class FixEventItemsDropFromMonstersUpdatePlugIn095d : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Event Items DropsFromMonsters 0.95d";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes event items that have DropsFromMonsters set to true, causing them to drop at level 0 instead of using their dedicated DropItemGroups.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixEventItemsDropFromMonsters095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 01, 27, 10, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        // Find event items that have DropsFromMonsters = true but also have dedicated DropItemGroups
        var eventItemsWithDropGroups = gameConfiguration.DropItemGroups
            .Where(dig => dig.PossibleItems.Any())
            .SelectMany(dig => dig.PossibleItems)
            .Where(item => item.Group is 13 or 14 && item.DropsFromMonsters)
            .Distinct()
            .ToList();

        foreach (var item in eventItemsWithDropGroups)
        {
            item.DropsFromMonsters = false;
        }
    }
}