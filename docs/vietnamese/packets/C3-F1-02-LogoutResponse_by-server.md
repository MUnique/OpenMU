# C3 F1 02 - LogoutResponse (server gửi)

## Được gửi khi nào

Sau khi server xử lý logout request.

## Hành động phía client

Tùy theo kết quả, client sẽ đóng game hoặc chuyển về màn hình chọn khác.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF1 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x02 | Packet header - sub packet type identifier |
| 4 | 1 | LogOutType |  | Type |

### Enum LogOutType

Mô tả cách người chơi muốn rời phiên game hiện tại.

| Value | Name | Description |
|-------|------|-------------|
| 0 | CloseGame | Người chơi muốn đóng game. |
| 1 | BackToCharacterSelection | Người chơi muốn quay về màn hình chọn nhân vật. |
| 2 | BackToServerSelection | Người chơi muốn quay về màn hình chọn server. |
