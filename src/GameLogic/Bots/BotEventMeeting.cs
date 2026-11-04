// <copyright file="BotEventMeeting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Where and until when a bot waits for players after it announced an opened event (see
/// <see cref="BotChatPlugIn"/>). A bot which posts "Blood Castle open, anyone going?" and keeps
/// hunting in another corner of the world could never be found - so it goes to the meeting point and
/// stays there until the entrance closes, ready to be invited into a party.
/// </summary>
/// <param name="Map">The map of the meeting point.</param>
/// <param name="NpcPosition">
/// The position of the event's entry NPC (Messenger of Archangel, Charon), next to which the bot waits;
/// <c>null</c> to wait anywhere in the map's safezone - Chaos Castle is entered with the ticket alone,
/// it has no entry NPC.
/// </param>
/// <param name="UntilUtc">When the event's entrance closes.</param>
internal sealed record BotEventMeeting(GameMapDefinition Map, Point? NpcPosition, DateTime UntilUtc);
