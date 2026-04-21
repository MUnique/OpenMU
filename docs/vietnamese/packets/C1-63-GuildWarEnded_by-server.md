# C1 63 - GuildWarEnded (server gửi)

## Được gửi khi nào

Cuộc chiến bang hội kết thúc.

## Hành động phía client

Cuộc chiến bang hội được hiển thị là đã kết thúc về phía khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x63 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | GuildWarResult |  | Kết quả |
| 4 | 8 | String |  | Tên bang hội |

### Enum GuildWarResult
Mô tả kết quả của cuộc chiến bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Lost | Cuộc chiến đã thất bại. |
| 1 | Won | Cuộc chiến đã thắng. |
| 2 | OtherGuildMasterCancelledWar | Cuộc chiến đã bị hủy bỏ bởi chủ bang hội khác. |
| 3 | CancelledWar | Cuộc chiến đã bị chính chủ bang hội hủy bỏ. |