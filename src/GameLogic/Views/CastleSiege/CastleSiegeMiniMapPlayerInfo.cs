// <copyright file="CastleSiegeMiniMapPlayerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The position of one player on the Castle Siege mini map.
/// </summary>
/// <param name="PositionX">The X coordinate.</param>
/// <param name="PositionY">The Y coordinate.</param>
public sealed record CastleSiegeMiniMapPlayerInfo(byte PositionX, byte PositionY);
