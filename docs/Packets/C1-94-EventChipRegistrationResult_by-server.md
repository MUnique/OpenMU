# C1 94 - EventChipRegistrationResult (by server)

## Is sent when

The player receives the result of registering Rena or Event Chips at the Golden Archer NPC.

## Causes the following actions on the client side

The client updates the Golden Archer interface with total registered count and remaining count in inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x94  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Result |
| 4 | 4 | IntegerLittleEndian |  | RegisteredCount |
| 8 | 2 | ShortLittleEndian |  | RemainingInventoryCount |