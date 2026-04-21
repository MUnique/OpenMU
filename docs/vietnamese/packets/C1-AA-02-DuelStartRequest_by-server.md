# C1 AA 02 - DuelStartRequest (server gửi)

## Được gửi khi nào

Sau khi một khách hàng khác gửi DuelStartRequest, để yêu cầu người chơi được yêu cầu phản hồi.

## Hành động phía client

Khách hàng hiển thị yêu cầu đấu tay đôi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Id người yêu cầu |
| 6 | 10 | String |  | Tên người yêu cầu |