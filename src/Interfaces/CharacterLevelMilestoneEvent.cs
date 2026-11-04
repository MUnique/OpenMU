// <copyright file="CharacterLevelMilestoneEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A character reached a notable level or master level.
/// </summary>
/// <remarks>
/// Names of configuration objects, like maps or monsters, are passed as the full value of a <see cref="LocalizedString"/>,
/// so that a receiver can translate them with <c>new LocalizedString(name).GetTranslation(culture)</c>.
/// </remarks>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="CharacterName">The name of the character.</param>
/// <param name="CharacterClassName">The name of the class of the character.</param>
/// <param name="Level">The reached level.</param>
/// <param name="IsMasterLevel">A value indicating whether <paramref name="Level"/> is a master level.</param>
public sealed record CharacterLevelMilestoneEvent(byte ServerId, DateTime TimestampUtc, string CharacterName, string CharacterClassName, int Level, bool IsMasterLevel)
    : GameEvent(ServerId, TimestampUtc);
