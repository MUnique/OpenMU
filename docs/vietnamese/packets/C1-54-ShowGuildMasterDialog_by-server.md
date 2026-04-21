# C1 54 - ShowGuildMasterDialog (server gửi)

## Được gửi khi nào

Sau khi người chơi tương tác với NPC Guild Master và đủ điều kiện tạo guild.

## Hành động phía client

Client mở hộp thoại guild master.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x54 | Packet header - packet type identifier |
