// <copyright file="FixEventItemsDropFromMonstersUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update fixes event items that have DropsFromMonsters set to true,
/// but should use dedicated DropItemGroups instead.
/// </summary>
public abstract class FixEventItemsDropFromMonstersUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Event Items DropsFromMonsters";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes event items that have DropsFromMonsters set to true, causing them to drop at level 0 instead of using their dedicated DropItemGroups.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 09, 01, 10, 0, 0, DateTimeKind.Utc);

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