// <copyright file="BotEventMeeting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Where and until when a bot waits for players after it announced an opened event, or after a
/// player whose party search it answered named the meeting place (see <see cref="BotChatPlugIn"/>).
/// A bot which posts "Blood Castle open, anyone going?" and keeps hunting in another corner of the
/// world could never be found - so it goes to the meeting point and stays there until the entrance
/// closes, ready to be invited into a party.
/// </summary>
/// <param name="Map">The map of the meeting point.</param>
/// <param name="NpcPosition">
/// The position next to which the bot waits: the event's entry NPC (Messenger of Archangel, Charon),
/// or the coordinates the player named; <c>null</c> to wait anywhere in the map's safezone - Chaos
/// Castle is entered with the ticket alone, it has no entry NPC.
/// </param>
/// <param name="UntilUtc">When the bot stops waiting, e.g. when the event's entrance closes.</param>
/// <param name="Gate">
/// The gate through which the bot enters the map: the one of the warp the player named ("devias3"), or
/// the one closest to the named coordinates - on a map like the Lost Tower, whose floors aren't connected
/// by walkable ground, the only way to get there; <c>null</c> for a spawn gate of the map.
/// </param>
internal sealed record BotEventMeeting(GameMapDefinition Map, Point? NpcPosition, DateTime UntilUtc, ExitGate? Gate = null);
