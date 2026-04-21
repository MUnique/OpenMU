# C3 33 - NpcItemSellResult (server gửi)

## Được gửi khi nào

Kết quả của yêu cầu bán mặt hàng trước đó.

## Hành động phía client

Số tiền được chỉ định được đặt trong kho của người chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x33 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Thành công |
| 4 | 4 | IntegerLittleEndian |  | Tiền bạc |