# C4 C5 - LetterSendRequest (by client)

## Is sent when

A player wants to send a letter to another players character.

## Causes the following actions on the server side

The letter is sent to the other character, if it exists and the player has the required money.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC4  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xC5  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | LetterId |
| 8 | 10 | String |  | Receiver |
| 18 | 60 | String |  | Title |
| 78 | 1 | Byte |  | Rotation |
| 79 | 1 | Byte |  | Animation |
| 80 | 2 | ShortLittleEndian |  | MessageLength |
| 82 |  | String |  | Message |