// <copyright file="WaitingItemReason.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The reason why a <see cref="WaitingItem"/> waits for space in the inventory.
/// </summary>
public enum WaitingItemReason
{
    /// <summary>
    /// The item found no free place when its storage was loaded,
    /// e.g. because the size of its definition was changed in the data.
    /// </summary>
    NoSpaceWhenLoaded,
}
