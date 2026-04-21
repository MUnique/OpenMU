# C1 3D - TradeCancel (client gửi)

## Được gửi khi nào

Khi người chơi muốn hủy giao dịch.

## Hành động phía server

Server hủy giao dịch và khôi phục trạng thái inventory trước khi trade.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x3D | Packet header - packet type identifier |
