# C1 F6 30 - AvailableQuestsRequest (client gửi)

## Được gửi khi nào

Khách hàng mở hộp thoại NPC nhiệm vụ và yêu cầu danh sách các nhiệm vụ có sẵn.

## Hành động phía server

Danh sách nhiệm vụ hiện có của NPC này được gửi lại (F60A).

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x30 | Tiêu đề gói - mã định danh loại gói phụ |