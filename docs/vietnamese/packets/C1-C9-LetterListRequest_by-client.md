# C1 C9 - LetterListRequest (client gửi)

## Được gửi khi nào

Máy khách trò chơi yêu cầu danh sách các chữ cái hiện tại.

## Hành động phía server

Máy chủ gửi danh sách các chữ cái có sẵn cho khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xC9 | Tiêu đề gói - mã định danh loại gói |