# C3 AA 02 - DuelStartResponse (client gửi)

## Được gửi khi nào

Một người chơi được yêu cầu bắt đầu đấu tay đôi với người chơi gửi.

## Hành động phía server

Tùy thuộc vào phản hồi mà máy chủ có bắt đầu cuộc đấu tay đôi hay không.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 17 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Phản ứng |
| 5 | 2 | ShortLittleEndian |  | Id người chơi |
| 7 | 10 | String |  | Tên người chơi |