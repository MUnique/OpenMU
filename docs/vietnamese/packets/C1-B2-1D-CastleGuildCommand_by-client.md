# C1 B2 1D - CastleGuildCommand (client gửi)

## Được gửi khi nào

Chủ bang hội đã gửi lệnh cho bang hội của mình trong sự kiện bao vây lâu đài.

## Hành động phía server

Lệnh được hiển thị trên bản đồ nhỏ của các thành viên bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x1D | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Đội; Đội số 0 đến 7. |
| 5 | 1 | Byte |  | Vị tríX |
| 6 | 1 | Byte |  | Vị tríY |
| 7 | 1 | Byte |  | Yêu cầu; 0 = Tấn công, 1 = Phòng thủ, 2 = Đợi |