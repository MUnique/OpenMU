# C1 56 - GuildCreationResult (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu tạo guild tại NPC Guild Master.

## Hành động phía client

Client hiển thị thông báo theo kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x56 | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 1 | GuildCreationErrorType |  | Error |

### Enum GuildCreationErrorType

Định nghĩa lỗi khi tạo guild.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | Không có lỗi. |
| 179 | GuildNameAlreadyTaken | Tên guild đã được sử dụng. |
