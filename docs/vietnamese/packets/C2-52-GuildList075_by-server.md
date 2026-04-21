# C2 52 - GuildList075 (server gửi)

## Được gửi khi nào

Sau khi client yêu cầu danh sách người chơi trong guild của họ, thường là khi mở hộp thoại guild ở client.

## Hành động phía client

Danh sách người chơi có sẵn phía client.

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
| 13 | GuildMember.Length * GuildMemberCount | Array of GuildMember |  | Members |

### Cấu trúc GuildMember

Chứa dữ liệu của một thành viên guild.

Độ dài: 12 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | ServerId |
| 11 | 1 | Byte |  | ServerId2 |
