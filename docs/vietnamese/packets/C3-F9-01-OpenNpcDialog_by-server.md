# C3 F9 01 - OpenNpcDialog (server gửi)

## Được gửi khi nào

Máy chủ xác nhận việc mở hộp thoại npc được yêu cầu.

## Hành động phía client

Máy khách mở hộp thoại của npc được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF9 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số Npc |
| 8 | 4 | IntegerLittleEndian |  | Điểm đóng góp Gens |