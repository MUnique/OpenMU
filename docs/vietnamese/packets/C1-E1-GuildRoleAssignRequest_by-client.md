# C1 E1 - GuildRoleAssignRequest (client gửi)

## Được gửi khi nào

Chủ bang hội muốn thay đổi vai trò của thành viên bang hội.

## Hành động phía server

Máy chủ thay đổi vai trò của thành viên bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xE1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 1 | Kiểu; Giá trị không xác định từ 1 đến 3. |
| 4 | 1 | ServerToClient.GuildMemberRole |  | Vai trò |
| 5 | 10 | String |  | Tên người chơi |