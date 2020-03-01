# C2 C0 - MessengerInitialization (by server)

## Is sent when

After entering the game with a character.

## Causes the following actions on the client side



## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xC0  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | LetterCount |
| 5 | 1 | Byte |  | MaximumLetterCount |
| 6 | 1 | Byte |  | FriendCount |
| 7 | Friend.Length * FriendCount | Array of Friend |  | Friends |

### Friend Structure

The structure which contains the friend name and online state.

Length: 11 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 1 | 1 | Byte |  | ServerId; The server id on which the player currently is online. 0xFF means offline. |