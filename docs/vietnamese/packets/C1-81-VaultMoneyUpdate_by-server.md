# C1 81 - VaultMoneyUpdate (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu chuyển tiền giữa kho tiền và kho.

## Hành động phía client

Ứng dụng khách trò chơi cập nhật giá trị tiền của kho tiền và kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x81 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Thành công |
| 4 | 4 | IntegerLittleEndian |  | VaultTiền |
| 8 | 4 | IntegerLittleEndian |  | Hàng tồn khoTiền |