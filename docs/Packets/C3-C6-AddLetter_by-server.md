# C3 C6 - AddLetter (by server)

## Is sent when

After a letter has been received or after the player entered the game with a character.

## Causes the following actions on the client side

The letter appears in the letter list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
<<<<<<< HEAD
| 1 | 1 |    Byte   |   79   | Packet header - length of the packet |
=======
| 1 | 1 |    Byte   |   107   | Packet header - length of the packet |
>>>>>>> origin/slave
| 2 | 1 |    Byte   | 0xC6  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | LetterIndex |
| 6 | 10 | String |  | SenderName |
| 16 | 30 | String |  | Timestamp |
<<<<<<< HEAD
| 46 | 32 | String |  | Subject |
| 78 | 1 | LetterState |  | State |
=======
| 46 | 60 | String |  | Subject |
| 106 | 1 | LetterState |  | State |
>>>>>>> origin/slave

### LetterState Enum

Describes the state of a letter.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Read | The letter was read before. |
| 1 | Unread | The letter wasn't read yet. |
| 2 | New | The letter is new (= was just sent by the sender) and wasn't read yet. It will notify the user about the received letter. |