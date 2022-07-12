// <copyright file="AddSummonedMonstersToScope.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// This part contains the custom indexer.
/// </summary>
public readonly ref partial struct AddSummonedMonstersToScopeRef
{
    /// <summary>
    /// Gets the final size, depending on the specified effects of each monster.
    /// </summary>
    public int FinalSize
    {
        get
        {
            // we can take the next potential index of the last character also as the size of the packet.
            _ = this.GetIndexOfMonster(this.MonsterCount - 1, out int finalSize);
            return finalSize;
        }
    }

    /// <summary>
    /// Gets the <see cref="SummonedMonsterDataRef" /> with the specified character index.
    /// </summary>
    /// <value>
    /// The <see cref="SummonedMonsterDataRef" />.
    /// </value>
    /// <param name="characterIndex">Index of the character.</param>
    /// <returns>The character data of the specified index.</returns>
    /// <remarks>
    /// It goes sequentially through the data. So when creating a new packet, it makes sense to create them sequentially in order of the index.
    /// </remarks>
    public SummonedMonsterDataRef this[int characterIndex]
    {
        get
        {
            var index = this.GetIndexOfMonster(characterIndex, out _);
            return new SummonedMonsterDataRef(this._data[index..]);
        }
    }

    private int GetIndexOfMonster(int characterIndex, out int nextIndex)
    {
        const int monstersStartIndex = 5;
        const int sizeWithoutEffects = 19;
        var currentIndex = monstersStartIndex;
        nextIndex = currentIndex;
        for (int i = 0; i <= characterIndex; i++)
        {
            currentIndex = nextIndex;
            var currentEffectCount = this._data[currentIndex + sizeWithoutEffects - 1];
            nextIndex += sizeWithoutEffects + currentEffectCount;
        }

        return currentIndex;
    }
}