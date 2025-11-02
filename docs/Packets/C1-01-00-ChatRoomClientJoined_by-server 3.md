# C1 01 00 - ChatRoomClientJoined (by server)

## Is sent when

This packet is sent by the server after another chat client joined the chat room.

## Causes the following actions on the client side

The client will add the client in its list (if over 2 clients are connected to the same room), or show its name in the title bar.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x01  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ClientIndex |
| 5 | 10 | String |  | Name |