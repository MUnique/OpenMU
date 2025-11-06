# C1 B2 02 - CastleSiegeRegistrationState (by server)

## Is sent when

When a player requests the castle siege registration state or after performing registration-related actions.

## Causes the following actions on the client side

The client shows whether the player's alliance is registered and the total marks submitted.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsRegistered; True if the player's alliance is registered for castle siege, false otherwise. |
| 5 | 4 | IntegerLittleEndian |  | TotalMarksSubmitted; The total number of guild marks submitted by the alliance. |