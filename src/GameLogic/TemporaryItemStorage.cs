// <copyright file="TemporaryItemStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// An <see cref="ItemStorage"/> which is not persisted, used for the temporary
/// storage of a <see cref="Player"/>, e.g. while an NPC or trade dialog is open.
/// </summary>
internal sealed class TemporaryItemStorage : ItemStorage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemporaryItemStorage"/> class.
    /// </summary>
    public TemporaryItemStorage()
    {
        this.Items = new List<Item>();
    }
}
