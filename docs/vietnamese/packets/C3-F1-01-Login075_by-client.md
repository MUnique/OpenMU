# C3 F1 01 - Login075 (client gửi)

## Được gửi khi nào

Khi người chơi cố gắng đăng nhập vào game.

## Hành động phía server

Server xác thực tên đăng nhập và mật khẩu được gửi. Nếu đúng, trạng thái người chơi sẽ tiến triển sang trạng thái đã đăng nhập.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 48 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF1 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 10 | Binary |  | Username; Tên đăng nhập được "mã hóa" bằng Xor3. |
| 14 | 10 | Binary |  | Password; Mật khẩu được "mã hóa" bằng Xor3. |
| 24 | 4 | IntegerBigEndian |  | TickCount |
| 28 | 3 | Binary |  | ClientVersion |
| 31 | 16 | Binary |  | ClientSerial |
