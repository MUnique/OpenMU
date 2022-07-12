// <copyright file="LegacyQuestStateList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// This part implements the indexer and size calculation for the message.
/// </summary>
public readonly ref partial struct LegacyQuestStateListRef
{
    private const int StatesPerByte = 4;
    private const int BitsPerState = 2;
    private const int ArrayStartIndex = 4;

    /// <summary>
    /// Gets or sets the <see cref="MUnique.OpenMU.Network.Packets.LegacyQuestState" /> at the specified index.
    /// </summary>
    /// <value>
    /// The <see cref="MUnique.OpenMU.Network.Packets.LegacyQuestState" />.
    /// </value>
    /// <param name="index">The index.</param>
    /// <returns>The <see cref="MUnique.OpenMU.Network.Packets.LegacyQuestState" /> at the specified index.</returns>
    public LegacyQuestState this[int index]
    {
        get => (LegacyQuestState)this._data.Slice(this.GetArrayIndex(index)).GetByteValue(BitsPerState, this.GetShift(index));

        set => this._data.Slice(this.GetArrayIndex(index)).SetByteValue((byte)value, BitsPerState, this.GetShift(index));
    }

    /// <summary>
    /// Gets the required size for the message with the specified state count.
    /// </summary>
    /// <param name="stateCount">The state count.</param>
    /// <returns>The required size for the message with the specified state count.</returns>
    public static int GetRequiredSize(int stateCount)
    {
        var result = ArrayStartIndex;
        result += stateCount / StatesPerByte;
        if (stateCount % StatesPerByte > 0)
        {
            result++;
        }

        return result;
    }

    private int GetArrayIndex(int stateIndex) => ArrayStartIndex + (stateIndex / StatesPerByte);

    private int GetShift(int stateIndex) => (stateIndex % StatesPerByte) * BitsPerState;
}