// <copyright file="ItemIdentifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The identifier for an item, including number and group.
/// </summary>
/// <param name="Number">The item number.</param>
/// <param name="Group">The item group.</param>
public record struct ItemIdentifier(short? Number, byte Group);