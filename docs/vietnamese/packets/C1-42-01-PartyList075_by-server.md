# C1 42 01 - PartyList075 (server gửi)

## Được gửi khi nào

Một người chơi đã vào party hoặc đã yêu cầu danh sách party hiện tại bằng cách mở hộp thoại party.

## Hành động phía client

Danh sách party được cập nhật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x42 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x01 | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Count |
| 5 | PartyMember.Length * Count | Array of PartyMember |  | Members |

### Cấu trúc PartyMember

Dữ liệu về một thành viên party.

Độ dài: 14 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | Index |
| 11 | 1 | Byte |  | MapId |
| 12 | 1 | Byte |  | PositionX |
| 13 | 1 | Byte |  | PositionY |
