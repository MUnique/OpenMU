# C3 F1 01 - LoginLongPassword (client gửi)

## Được gửi khi nào

Khi người chơi thử đăng nhập vào game.

## Hành động phía server

Server xác thực username/password được gửi lên. Nếu hợp lệ, trạng thái người
chơi được chuyển sang đăng nhập thành công.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 60 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF1 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 10 | Binary |  | Username; username được "mã hóa" bằng Xor3. |
| 14 | 20 | Binary |  | Password; password được "mã hóa" bằng Xor3. |
| 34 | 4 | IntegerBigEndian |  | TickCount |
| 38 | 5 | Binary |  | ClientVersion |
| 43 | 16 | Binary |  | ClientSerial |
