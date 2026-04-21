# C1 E9 - RequestAllianceList (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại danh sách liên minh.

## Hành động phía server

Máy chủ trả lời kèm theo danh sách các bang hội của liên minh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xE9 | Tiêu đề gói - mã định danh loại gói |