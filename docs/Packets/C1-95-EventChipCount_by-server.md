# C1 95 - EventChipCount (by server)

## Is sent when

After event chips were registered.

## Causes the following actions on the client side

The client updates the shown count of registered event chips.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x95  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Count; -1 keeps the current value. |