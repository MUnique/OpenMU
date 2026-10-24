// <copyright file="CastleSiegeMiniMapNpcInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The position of one Castle Siege gate or Guardian Statue on the mini map.
/// </summary>
/// <param name="IsGate">Whether the structure is a gate; otherwise, a Guardian Statue.</param>
/// <param name="PositionX">The X coordinate.</param>
/// <param name="PositionY">The Y coordinate.</param>
public sealed record CastleSiegeMiniMapNpcInfo(bool IsGate, byte PositionX, byte PositionY);
