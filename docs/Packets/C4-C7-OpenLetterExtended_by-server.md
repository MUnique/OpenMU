# C4 C7 - OpenLetterExtended (by server)

## Is sent when

After the player requested to read a letter.

## Causes the following actions on the client side

The letter is opened in a new dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC4  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xC7  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | LetterIndex |
| 6 | 42 | Binary |  | SenderAppearance |
| 48 | 1 | Byte |  | Rotation |
| 49 | 1 | Byte |  | Animation |
| 50 |  | String |  | Message |