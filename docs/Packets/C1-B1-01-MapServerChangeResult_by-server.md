# C1 B1 01 - MapServerChangeResult (by server)

## Is sent when

After the authentication at the new game server.

## Causes the following actions on the client side

The 1.04d client does not read any payload of this packet.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |