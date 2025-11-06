# C1 B2 04 - CastleSiegeMarkSubmitted (by server)

## Is sent when

After a player submitted a guild mark (Sign of Lord item) for castle siege registration.

## Causes the following actions on the client side

The client shows the total number of submitted marks for the player's alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | TotalMarksSubmitted; The total number of guild marks submitted by the alliance. |