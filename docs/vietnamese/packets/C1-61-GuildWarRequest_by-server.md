# C1 61 - GuildWarRequest (server gửi)

## Được gửi khi nào

Một chủ bang hội đã yêu cầu một cuộc chiến bang hội chống lại một bang hội khác.

## Hành động phía client

Chủ bang hội của bang hội khác nhận được yêu cầu này.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x61 | Tiêu đề gói - mã định danh loại gói |
| 3 | 8 | String |  | Tên bang hội |
| 11 | 1 | GuildWarType |  | Type |

### Enum GuildWarType
Mô tả loại cuộc chiến bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Một cuộc chiến bang hội bình thường. |
| 1 | Soccer | Một trận đấu bóng đá của bang hội. |