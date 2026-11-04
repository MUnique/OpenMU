# C1 C8 - RemoveLetter (by server)

## Is sent when

After a letter has been deleted by the request of the player.

## Causes the following actions on the client side

The letter is removed from the letter list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC8  | Packet header - packet type identifier |
| 3 | 1 | Boolean | true | RequestSuccessful |
| 4 | 2 | ShortLittleEndian |  | LetterIndex |