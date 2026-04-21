# C1 F3 02 - DeleteCharacter (client gửi)

## Được gửi khi nào

Khi client đang ở màn hình chọn nhân vật và người chơi yêu cầu xóa một nhân
vật đã tồn tại.

## Hành động phía server

Server kiểm tra security code và kiểm tra nhân vật có tồn tại không.
Nếu hợp lệ, server xóa nhân vật khỏi account và gửi response kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 24 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x02 | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name; tên nhân vật cần xóa. |
| 14 |  | String |  | SecurityCode; mã bảo mật (thường 7 bytes). Một số client/server dùng password account tại đây (tối đa 20 bytes). OpenMU dùng security code và không giới hạn đúng 7 bytes. |
