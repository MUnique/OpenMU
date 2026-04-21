# C1 90 - DevilSquareEnterResult (server gửi)

## Được gửi khi nào

Khi người chơi yêu cầu vào mini game Devil Square qua NPC Charon.

## Hành động phía client

Nếu thất bại, client hiển thị thông báo lỗi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x90 | Packet header - packet type identifier |
| 3 | 1 | EnterResult |  | Result |

### Enum EnterResult

Kết quả yêu cầu vào event.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | Vào event thành công. |
| 1 | Failed | Thất bại, ví dụ thiếu vé hoặc không đúng khoảng level. |
| 2 | NotOpen | Event chưa mở. |
| 3 | CharacterLevelTooHigh | Level nhân vật quá cao so với level event yêu cầu. |
| 4 | CharacterLevelTooLow | Level nhân vật quá thấp so với level event yêu cầu. |
| 5 | Full | Event đã đầy. |
