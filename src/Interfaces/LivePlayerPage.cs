// <copyright file="LivePlayerPage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>A bounded page of live characters, filtered before paging.</summary>
/// <param name="Items">The matching page.</param>
/// <param name="Total">The total number of matches.</param>
public sealed record LivePlayerPage(IReadOnlyList<LivePlayerInfo> Items, int Total);
