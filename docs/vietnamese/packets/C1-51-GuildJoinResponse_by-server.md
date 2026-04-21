# C1 51 - GuildJoinResponse (server gửi)

## Được gửi khi nào

Sau khi guild master phản hồi yêu cầu gia nhập guild của người chơi.
Packet này được gửi lại cho người đã gửi yêu cầu.

## Hành động phía client

Người chơi nhận thông báo kết quả tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x51 | Packet header - packet type identifier |
| 3 | 1 | GuildJoinRequestResult |  | Result |

### Enum GuildJoinRequestResult

Kết quả yêu cầu gia nhập guild.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Refused | Bị guild master từ chối. |
| 1 | Accepted | Được guild master chấp nhận. |
| 2 | GuildFull | Guild đã đầy. |
| 3 | Disconnected | Guild master đã mất kết nối. |
| 4 | NotTheGuildMaster | Người được yêu cầu không phải guild master. |
| 5 | AlreadyHaveGuild | Người chơi đã có guild. |
| 6 | GuildMasterOrRequesterIsBusy | Guild master hoặc người yêu cầu đang bận (ví dụ có yêu cầu khác hoặc đang guild war). |
| 7 | MinimumLevel6 | Người yêu cầu cần tối thiểu level 6. |
