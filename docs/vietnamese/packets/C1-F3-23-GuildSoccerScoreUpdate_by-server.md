# C1 F3 23 - GuildSoccerScoreUpdate (server gửi)

## Được gửi khi nào

Bất cứ khi nào tỷ số của trận đấu bóng đá thay đổi và khi bắt đầu trận đấu.

## Hành động phía client

Điểm số được cập nhật trên giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 22 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x23 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 8 | String |  | Tên đội đỏ |
| 12 | 1 | Byte |  | Đội ĐỏMục Tiêu |
| 13 | 8 | String |  | Tên nhóm xanh |
| 21 | 1 | Byte |  | BlueTeamMục tiêu |