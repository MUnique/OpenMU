# C1 93 - MiniGameScoreTable (server gửi)

## Được gửi khi nào

Một mini game kết thúc và bảng điểm được gửi tới người chơi.

## Hành động phía client

Bảng điểm được hiển thị tại khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x93 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Xếp hạng người chơi |
| 4 | 1 | Byte |  | Kết quảĐếm |
| 5 | ResultItem.Length * ResultCount | Array of ResultItem |  | Kết quả |

### Cấu trúc ResultItem
Kết quả của một người chơi.

Độ dài: 24 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Tên người chơi |
| 12 | 4 | IntegerLittleEndian |  | Tổng điểm |
| 16 | 4 | IntegerLittleEndian |  | Tiền thưởngKinh nghiệm |
| 20 | 4 | IntegerLittleEndian |  | Tiền thưởngTiền |