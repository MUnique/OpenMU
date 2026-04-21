# C1 43 - RemovePartyMember (server gửi)

## Được gửi khi nào

Khi một thành viên bị loại khỏi party mà người chơi hiện tại đang tham gia.

## Hành động phía client

Xóa thành viên có index tương ứng khỏi danh sách party trên giao diện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x43 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Index |
