# C2 52 - GuildList (server gửi)

## Được gửi khi nào

Sau khi client yêu cầu danh sách thành viên guild, thường khi mở cửa sổ guild.

## Hành động phía client

Danh sách thành viên guild được cập nhật trên client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Packet header - độ dài packet |
| 3 | 1 | Byte | 0x52 | Packet header - packet type identifier |
| 4 | 1 | Boolean |  | IsInGuild |
| 5 | 1 | Byte |  | GuildMemberCount |
| 8 | 4 | IntegerLittleEndian |  | TotalScore |
| 12 | 1 | Byte |  | CurrentScore |
| 13 | 8 | String |  | RivalGuildName |
| 24 | GuildMember.Length * GuildMemberCount | Array of GuildMember |  | Members |

### Cấu trúc GuildMember

Chứa dữ liệu của một thành viên guild.

Độ dài: 13 bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | ServerId |
| 11 | 1 | Byte |  | ServerId2 |
| 12 | 1 | GuildMemberRole |  | Role |

### Enum GuildMemberRole

Vai trò của thành viên guild.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | Thành viên thường, không có quyền đặc biệt. |
| 32 | BattleMaster | Thành viên là battle master. |
| 64 | AssistantMaster | Thành viên là phó bang chủ. |
| 128 | GuildMaster | Thành viên là bang chủ. |
| 255 | Undefined | Nhân vật không thuộc guild nên vai trò không xác định. |
