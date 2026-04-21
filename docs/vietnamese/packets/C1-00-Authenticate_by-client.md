# C1 00 - Authenticate (client gửi)

## Được gửi khi nào

Packet này được client gửi sau khi kết nối server để xác thực phiên làm việc.

## Hành động phía server

Server kiểm tra token. Nếu hợp lệ, client sẽ được thêm vào chat room đã yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x00 | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | RoomId |
| 6 | 10 | Binary |  | Token; token (số nguyên) được format thành chuỗi và "mã hóa" bằng khóa XOR 3 byte (FC CF AB). |
