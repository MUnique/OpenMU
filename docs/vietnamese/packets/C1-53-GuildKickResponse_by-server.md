# C1 53 - GuildKickResponse (server gửi)

## Được gửi khi nào

Sau khi guild master gửi yêu cầu kick thành viên và server xử lý xong yêu cầu đó.

## Hành động phía client

Client hiển thị message theo kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x53 | Packet header - packet type identifier |
| 3 | 1 | GuildKickSuccess |  | Result |

### Enum GuildKickSuccess

Kết quả của yêu cầu kick guild.

| Value | Name | Description |
|-------|------|-------------|
| 0 | FailedPasswordIncorrect | Thất bại do sai mật khẩu. |
| 1 | KickSucceeded | Kick thành công. |
| 2 | KickFailedBecausePlayerIsNotGuildMaster | Thất bại vì người gửi không phải guild master. |
| 3 | Failed | Kick thất bại. |
| 4 | GuildDisband | Guild đã bị giải tán. |
| 5 | GuildMemberWithdrawn | Thành viên đã rời guild. |
