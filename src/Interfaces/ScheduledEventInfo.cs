// <copyright file="ScheduledEventInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// An event of a game server which runs according to a schedule, like a mini game, an invasion or the happy hour.
/// </summary>
/// <param name="PlugInId">The identifier of the plugin which runs the event; it stays the same across versions and languages.</param>
/// <param name="Name">The english name of the event.</param>
/// <param name="NextStartUtc">The next start time, in UTC.</param>
public sealed record ScheduledEventInfo(Guid PlugInId, string Name, DateTime NextStartUtc);
