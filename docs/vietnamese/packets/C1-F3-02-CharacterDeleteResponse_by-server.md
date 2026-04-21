# C1 F3 02 - CharacterDeleteResponse (server gửi)

## Được gửi khi nào

Sau khi server xử lý yêu cầu xóa nhân vật từ client.

## Hành động phía client

Nếu thành công, nhân vật bị xóa khỏi màn hình chọn nhân vật.
Nếu thất bại, hiển thị thông báo lỗi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x02 | Packet header - sub packet type identifier |
| 4 | 1 | CharacterDeleteResult |  | Result |

### Enum CharacterDeleteResult

Kết quả yêu cầu xóa nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unsuccessful | Xóa không thành công. |
| 1 | Successful | Xóa thành công. |
| 2 | WrongSecurityCode | Xóa thất bại do sai security code. |
