// <copyright file="FixMiniGameZenDropsUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update configures which mini game maps and monsters drop no zen.
/// Before, the zen of every monster killed in a mini game was added to the inventory of the killer,
/// instead of being dropped. Now it's dropped like anywhere else, except on the maps and by the monsters
/// which give no zen in the original game, e.g. in the Devil Square.
/// </summary>
public abstract class FixMiniGameZenDropsUpdatePlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.FixMiniGameZenDropsUpdatePlugIn_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.FixMiniGameZenDropsUpdatePlugIn_Description;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 22, 22, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the numbers of the maps whose monsters drop no zen.
    /// </summary>
    protected abstract IReadOnlyCollection<short> MapNumbersWithoutZen { get; }

    /// <summary>
    /// Gets the numbers of the monsters which drop no zen.
    /// </summary>
    protected virtual IReadOnlyCollection<short> MonsterNumbersWithoutZen => [];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var map in gameConfiguration.Maps.Where(map => this.MapNumbersWithoutZen.Contains(map.Number)))
        {
            foreach (var moneyGroup in map.DropItemGroups.Where(group => group.ItemType == SpecialItemType.Money).ToList())
            {
                map.DropItemGroups.Remove(moneyGroup);
            }
        }

        foreach (var monster in gameConfiguration.Monsters.Where(monster => this.MonsterNumbersWithoutZen.Contains(monster.Number)))
        {
            monster.DropsNoZen = true;
        }

        return ValueTask.CompletedTask;
    }
}
