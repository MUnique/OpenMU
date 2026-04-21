# C1 44 - PartyHealthUpdate (server gửi)

## Được gửi khi nào

Được gửi định kỳ khi trạng thái máu của party thay đổi.

## Hành động phía client

Cập nhật danh sách máu party trên giao diện.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x44 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Count |
| 4 | PartyMemberHealth.Length * Count | Array of PartyMemberHealth |  | Members |

### Cấu trúc PartyMemberHealth

Máu của một thành viên party.

Độ dài: 1 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 bit | Byte |  | Index |
| 0 | 4 bit | Byte |  | Value; giá trị từ 0 đến 10 biểu thị phần trăm máu của người chơi. 10 nghĩa là 100% máu hiện tại so với máu tối đa. |
