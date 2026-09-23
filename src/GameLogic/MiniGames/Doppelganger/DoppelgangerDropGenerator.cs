// <copyright file="DoppelgangerDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using System.Threading;

/// <summary>
/// The drop generator of the doppelganger event. It delegates to the drop generator of the
/// game, but suppresses the drop of an interim reward chest, when larvae came out of it instead.
/// </summary>
/// <remarks>
/// The drop of a killed object is generated a moment after its death, so the context decides
/// about the content of the chest first and tells this generator to suppress the drop.
/// </remarks>
public sealed class DoppelgangerDropGenerator : IDropGenerator
{
    private readonly IDropGenerator _dropGenerator;
    private readonly short _interimRewardChestNumber;
    private int _suppressedInterimChestDrops;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerDropGenerator"/> class.
    /// </summary>
    /// <param name="dropGenerator">The drop generator of the game.</param>
    /// <param name="interimRewardChestNumber">The number of the interim reward chest.</param>
    public DoppelgangerDropGenerator(IDropGenerator dropGenerator, short interimRewardChestNumber)
    {
        this._dropGenerator = dropGenerator;
        this._interimRewardChestNumber = interimRewardChestNumber;
    }

    /// <summary>
    /// Suppresses the drop of the next opened interim reward chest.
    /// </summary>
    public void SuppressNextInterimChestDrop()
    {
        Interlocked.Increment(ref this._suppressedInterimChestDrops);
    }

    /// <inheritdoc />
    public ValueTask<(IEnumerable<Item> Items, uint? Money)> GenerateItemDropsAsync(MonsterDefinition monster, int gainedExperience, Player player)
    {
        if (monster.Number == this._interimRewardChestNumber && this.TryConsumeSuppressedDrop())
        {
            return ValueTask.FromResult<(IEnumerable<Item> Items, uint? Money)>(([], null));
        }

        return this._dropGenerator.GenerateItemDropsAsync(monster, gainedExperience, player);
    }

    /// <inheritdoc />
    public Item? GenerateItemDrop(DropItemGroup group)
    {
        return this._dropGenerator.GenerateItemDrop(group);
    }

    /// <inheritdoc />
    public (Item? Item, uint? Money, ItemDropEffect DropEffect) GenerateItemDrop(IEnumerable<DropItemGroup> groups)
    {
        return this._dropGenerator.GenerateItemDrop(groups);
    }

    private bool TryConsumeSuppressedDrop()
    {
        var current = Volatile.Read(ref this._suppressedInterimChestDrops);
        while (current > 0)
        {
            var previous = Interlocked.CompareExchange(ref this._suppressedInterimChestDrops, current - 1, current);
            if (previous == current)
            {
                return true;
            }

            current = previous;
        }

        return false;
    }
}
