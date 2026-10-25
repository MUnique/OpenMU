// <copyright file="LivePlayerResponse.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using MUnique.OpenMU.Interfaces;

/// <summary>Keeps an absent live player as a JSON response instead of an empty HTTP 204.</summary>
/// <param name="Player">The live player, or null when unavailable.</param>
public sealed record LivePlayerResponse(LivePlayerInfo? Player);
