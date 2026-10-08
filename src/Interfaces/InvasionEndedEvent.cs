// <copyright file="InvasionEndedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// An invasion ended, e.g. the golden invasion.
/// </summary>
/// <remarks>
/// Names of configuration objects, like maps or monsters, are passed as the full value of a <see cref="LocalizedString"/>,
/// so that a receiver can translate them with <c>new LocalizedString(name).GetTranslation(culture)</c>.
/// </remarks>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="InvasionId">The identifier of the plugin which runs the invasion. It identifies the kind of invasion.</param>
/// <param name="InvasionName">The name of the invasion, in the neutral language.</param>
/// <param name="MapNames">The names of the maps which are announced as invaded.</param>
public sealed record InvasionEndedEvent(byte ServerId, DateTime TimestampUtc, Guid InvasionId, string InvasionName, IReadOnlyList<string> MapNames)
    : GameEvent(ServerId, TimestampUtc);
