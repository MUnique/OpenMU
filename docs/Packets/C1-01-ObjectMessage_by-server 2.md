# C1 01 - ObjectMessage (by server)

## Is sent when

The server wants to show a message above any kind of character, even NPCs.

## Causes the following actions on the client side

The message is shown above the character.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x01  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 |  | String |  | Message |