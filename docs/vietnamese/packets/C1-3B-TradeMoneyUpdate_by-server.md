# C1 3B - TradeMoneyUpdate (server gửi)

## Được gửi khi nào

Thông báo này được gửi khi đối tác thương mại đặt một số tiền nhất định (cũng bằng 0) vào giao dịch.

## Hành động phía client

Nó ghi đè tất cả các giá trị tiền đã gửi trước đó.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3B | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | TiềnSố tiền |