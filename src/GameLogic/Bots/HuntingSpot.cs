// <copyright file="HuntingSpot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A hunting ground a bot has claimed, together with the map it lies on.
/// </summary>
/// <remarks>
/// This is a reference type on purpose: claims are read by other bots' timers without a lock, and
/// reference assignment is atomic. A small struct could be read torn across threads.
/// </remarks>
/// <param name="Position">The position of the ground.</param>
/// <param name="MapNumber">The number of the map the ground lies on.</param>
internal sealed record HuntingSpot(Point Position, short MapNumber);
