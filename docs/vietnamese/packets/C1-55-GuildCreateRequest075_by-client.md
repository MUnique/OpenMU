# C1 55 - GuildCreateRequest075 (client gửi)

## Được gửi khi nào

Khi người chơi muốn thành lập bang hội.

## Hành động phía server

Bang hội được thành lập và người chơi được chọn làm chủ bang hội mới của bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 43 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x55 | Tiêu đề gói - mã định danh loại gói |
| 3 | 8 | String |  | Tên bang hội |
| 11 | 32 | Binary |  | Biểu tượng bang hội; Biểu tượng bang hội ở định dạng bitmap tùy chỉnh. Nó hỗ trợ 16 màu (một màu trong suốt) cho mỗi pixel và có kích thước 8 * 8 pixel. |