# C1 F3 11 - SkillRemoved075 (server gửi)

## Được gửi khi nào

Sau khi một skill bị gỡ khỏi danh sách skill, ví dụ do tháo trang bị.

## Hành động phía client

Skill bị gỡ khỏi danh sách skill phía client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x11 | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 0 | Flag |
| 5 | 1 | Byte |  | SkillIndex |
| 6 | 2 | ShortBigEndian |  | SkillNumberAndLevel |
