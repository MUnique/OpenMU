# C1 F7 02 - ImperialGuardianEnterResult (by server)

## Is sent when

The player requested to enter the imperial guardian event, or entered the next zone of it.

## Causes the following actions on the client side

The client shows a message when entering failed. On success, it remembers the day and zone for the timer and the result, and sets the weather of the map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | EnterResult |  | Result; The result of the request. |
| 5 | 1 | Byte |  | Day; The day of the week, from 1 (monday) to 7 (sunday). The client shows it as the round. |
| 6 | 1 | Byte |  | Zone; The zone, starting at 1. |
| 7 | 1 | WeatherType |  | Weather; The weather of the map. |
| 8 | 4 | IntegerLittleEndian |  | RemainingMilliseconds; The remaining time in milliseconds. When the result is NotOpen, the client shows the minutes until the event can be entered. The field is aligned to 4 bytes, because the client structure is not packed. |

### EnterResult Enum

The result of an enter request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The player entered the event. |
| 1 | NotOpen | The event can not be entered yet. |
| 2 | MissingTicket | The player has no Gaion's Order or Complete Secromicon. |
| 3 | Full | The zone is full. |
| 4 | ZoneTimeRemaining | There is still time remaining in this zone. |
| 5 | PartyRequired | The player can only enter as a member of a party. |
| 6 | CharacterLevelTooLow | The character level is too low. The client doesn't show a message. |

### WeatherType Enum

The weather of the map.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Sun | The sun shines. |
| 1 | Rain | It rains. |
| 2 | Fog | There is fog. |
| 3 | Storm | There is a storm. |