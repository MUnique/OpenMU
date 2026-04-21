# C1 A1 - LegacyQuestStateDialog (server gửi)

## Được gửi khi nào

Khi người chơi nhấp vào nhiệm vụ npc.

## Hành động phía client

Ứng dụng khách trò chơi hiển thị các bước tiếp theo trong hộp thoại nhiệm vụ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Chỉ mục nhiệm vụ |
| 4 | 1 | Byte |  | Tình trạng; Đây là byte hoàn chỉnh với trạng thái của bốn nhiệm vụ trong cùng một byte. |