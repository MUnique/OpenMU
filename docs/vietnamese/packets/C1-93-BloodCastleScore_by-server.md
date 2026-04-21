# C1 93 - BloodCastleScore (server gửi)

## Được gửi khi nào

Mini game lâu đài máu đã kết thúc và điểm số của người chơi được gửi đến người chơi.

## Hành động phía client

Điểm số được hiển thị tại khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 29 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x93 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | Thành công |
| 4 | 1 | Byte | 0xFF | Type |
| 5 | 10 | String |  | Tên người chơi |
| 17 | 4 | IntegerLittleEndian |  | Tổng điểm |
| 21 | 4 | IntegerLittleEndian |  | Tiền thưởngKinh nghiệm |
| 25 | 4 | IntegerLittleEndian |  | Tiền thưởngTiền |