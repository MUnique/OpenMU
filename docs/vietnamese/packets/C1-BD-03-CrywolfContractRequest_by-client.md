# C1 BD 03 - CrywolfContractRequest (client gửi)

## Được gửi khi nào

Một người chơi muốn lập hợp đồng tại bức tượng người sói cho sự kiện người sói.

## Hành động phía server

Máy chủ cố gắng ký hợp đồng với người chơi và bức tượng được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBD | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id tượng |