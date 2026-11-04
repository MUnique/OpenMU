# C1 BF 13 - DoppelgangerResult (by server)

## Is sent when

The doppelganger event ended for the player.

## Causes the following actions on the client side

The client stops the timer and the event music, and shows a message box with the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x13  | Packet header - sub packet type identifier |
| 4 | 1 | ResultType |  | Result |
| 8 | 4 | IntegerLittleEndian |  | RewardExperience; The experience which the player got as reward. It is not shown by the client. The field is aligned to 4 bytes, because the client structure is not packed. |

### ResultType Enum

The result of the doppelganger event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The party successfully defended the magic circle. |
| 1 | Failed | The player failed, e.g. because the character died or left the event map. |
| 2 | MonstersReachedMagicCircle | The defense failed, because too many monsters reached the magic circle. |