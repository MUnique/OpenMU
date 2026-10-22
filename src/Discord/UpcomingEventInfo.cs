// <copyright file="UpcomingEventInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// An event which starts soon.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="StartsAtUtc">The start time, in UTC.</param>
public sealed record UpcomingEventInfo(string Name, DateTime StartsAtUtc);
