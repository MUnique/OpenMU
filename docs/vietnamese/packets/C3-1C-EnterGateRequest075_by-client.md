# C3 1C - EnterGateRequest075 (client gửi)

## Được gửi khi nào

Thông thường: khi người chơi đi vào một vùng trên bản đồ được cấu hình là cổng (gate) trong dữ liệu client. Trường hợp đặc biệt với pháp sư (wizard): packet này cũng được dùng cho skill teleport. Khi đó GateNumber là 0 và toạ đích được chỉ định.

## Hành động phía server

Nếu người chơi được phép đi qua “cổng”, nhân vật sẽ được chuyển tới khu vực cổng ra tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x1C | Packet header - packet type identifier |
| 3 | 1 | Byte |  | GateNumber |
| 4 | 1 | Byte |  | TeleportTargetX |
| 5 | 1 | Byte |  | TeleportTargetY |
