// <copyright file="CastleSiegeStatePeriod.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A calculated Castle Siege state period.
/// </summary>
/// <param name="State">The state.</param>
/// <param name="StartUtc">The UTC start time.</param>
/// <param name="EndUtc">The UTC end time.</param>
internal readonly record struct CastleSiegeStatePeriod(CastleSiegeState State, DateTime StartUtc, DateTime EndUtc);
