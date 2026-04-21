# C2 5B - AssignCharacterToGuild075 (server gửi)

## Được gửi khi nào

Máy chủ muốn chỉ định rõ ràng một người chơi vào bang hội, ví dụ: khi hai người chơi gặp nhau và một trong số họ là thành viên bang hội.

## Hành động phía client

Những người chơi thuộc bang hội được hiển thị là người chơi của bang hội. Nếu ứng dụng khách trò chơi chưa gặp người chơi của bang hội này, nó sẽ gửi một yêu cầu khác để lấy thông tin bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x5B | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | Số lượng người chơi |
| 5 | GuildMemberRelation.Length * PlayerCount | Array of GuildMemberRelation |  | Thành viên |

### Cấu trúc GuildMemberRelation
Mối quan hệ giữa bang hội và thành viên.

Độ dài: 4 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id người chơi |
| 2 | 2 | ShortBigEndian |  | Id bang hội |