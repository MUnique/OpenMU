# C1 42 01 - PartyList (server gửi)

## Được gửi khi nào

Khi người chơi vừa vào party hoặc yêu cầu danh sách party hiện tại (thường khi
mở cửa sổ party).

## Hành động phía client

Cập nhật danh sách thành viên party.

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

Dữ liệu một thành viên party.

Độ dài: 24 bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | Index |
| 11 | 1 | Byte |  | MapId |
| 12 | 1 | Byte |  | PositionX |
| 13 | 1 | Byte |  | PositionY |
| 16 | 4 | IntegerLittleEndian |  | CurrentHealth |
| 20 | 4 | IntegerLittleEndian |  | MaximumHealth |
