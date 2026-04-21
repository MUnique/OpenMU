# C3 1E - AreaSkill095 (client gửi)

## Được gửi khi nào

Khi người chơi thực hiện một skill ảnh hưởng một vùng trên bản đồ.

## Hành động phía server

Packet được chuyển tiếp tới tất cả người chơi xung quanh để animation có thể nhìn thấy. Trong implementation server gốc, skill tấn công chưa gây sát thương tại bước này — có các packet hit riêng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x1E | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillIndex; Chỉ số skill trong danh sách skill. |
| 4 | 1 | Byte |  | TargetX |
| 5 | 1 | Byte |  | TargetY |
| 6 | 1 | Byte |  | Rotation |
