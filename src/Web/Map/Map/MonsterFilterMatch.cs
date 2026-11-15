// <copyright file="MonsterFilterMatch.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

/// <summary>
/// A monster which matches the monster filter of the live map.
/// </summary>
/// <param name="Id">The id of the monster on the map.</param>
/// <param name="Name">The name of the monster.</param>
/// <param name="X">The x coordinate of the monster.</param>
/// <param name="Y">The y coordinate of the monster.</param>
public sealed record MonsterFilterMatch(int Id, string Name, int X, int Y);
