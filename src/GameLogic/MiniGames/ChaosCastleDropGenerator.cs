// <copyright file="ChaosCastleDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Immutable;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// An <see cref="IDropGenerator"/> especially for Chaos Castle.
/// It will drop a predefined amount of jewels during the whole event,
/// and nothing more.
/// </summary>
public class ChaosCastleDropGenerator : IDropGenerator
{
    private static readonly IImmutableList<(int Blesses, int Souls)> MonsterJewelDropsPerLevel = new List<(int Blesses, int Souls)>
    {
        new(0, 0), // Dummy
        new(0, 2), // Chaos Castle 1
        new(1, 1), // Chaos Castle 2
        new(1, 2), // Chaos Castle 3
        new(1, 2), // Chaos Castle 4
        new(2, 1), // Chaos Castle 5
        new(2, 2), // Chaos Castle 6
        new(2, 3), // Chaos Castle 7
    }.ToImmutableList();

    private readonly Dictionary<int, ItemDefinition> _monsterDrops = new();
    private readonly IGameContext _gameContext;
    private int _killedMonsters;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastleDropGenerator"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="context">The context.</param>
    /// <param name="spawnedMonstersCount">The spawned monsters count.</param>
    public ChaosCastleDropGenerator(IGameContext gameContext, ChaosCastleContext context, int spawnedMonstersCount)
    {
        this._gameContext = gameContext;

        // TODO: This should rather be configurable...
        // Inactive jewels are not dropped.
        var blessDefinition = this._gameContext.Configuration.Items.FirstOrDefault(item => item is { Group: 14, Number: 13, IsActive: true });
        var soulDefinition = this._gameContext.Configuration.Items.FirstOrDefault(item => item is { Group: 14, Number: 14, IsActive: true });
        var drops = MonsterJewelDropsPerLevel[context.Definition.GameLevel];
        AddItems(drops.Blesses, blessDefinition);
        AddItems(drops.Souls, soulDefinition);

        void AddItems(int count, ItemDefinition? definition)
        {
            if (definition is null)
            {
                return;
            }

            // Each monster drops one item at most, so there can't be more drops than monsters.
            for (var i = 0; i < count && this._monsterDrops.Count < spawnedMonstersCount; i++)
            {
                bool assigned;
                do
                {
                    var randomKillCount = Rand.NextInt(1, spawnedMonstersCount + 1);
                    assigned = this._monsterDrops.TryAdd(randomKillCount, definition);
                }
                while (!assigned);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask<(IEnumerable<Item> Items, uint? Money)> GenerateItemDropsAsync(MonsterDefinition monster, int gainedExperience, Player player)
    {
        var killCount = Interlocked.Increment(ref this._killedMonsters);
        if (this._monsterDrops.TryGetValue(killCount, out var drop))
        {
            var item = new TemporaryItem
            {
                Definition = drop,
                Durability = 1,
            };

            return (item.GetAsEnumerable(), null);
        }

        return (Enumerable.Empty<Item>(), null);
    }

    /// <inheritdoc />
    public Item? GenerateItemDrop(DropItemGroup group)
    {
        return this._gameContext.DropGenerator.GenerateItemDrop(group);
    }

    /// <inheritdoc />
    public (Item? Item, uint? Money, ItemDropEffect DropEffect) GenerateItemDrop(IEnumerable<DropItemGroup> groups)
    {
        return this._gameContext.DropGenerator.GenerateItemDrop(groups);
    }
}