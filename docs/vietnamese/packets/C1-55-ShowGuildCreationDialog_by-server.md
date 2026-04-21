# C1 55 - ShowGuildCreationDialog (server gửi)

## Được gửi khi nào

Sau khi người chơi bắt đầu nói chuyện với NPC chủ bang hội và người chơi tiến hành tạo bang hội.

## Hành động phía client

Khách hàng hiển thị hộp thoại tạo bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x55 | Tiêu đề gói - mã định danh loại gói |