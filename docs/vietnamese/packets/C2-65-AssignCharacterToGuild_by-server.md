# C2 65 - AssignCharacterToGuild (server gửi)

## Được gửi khi nào

Máy chủ muốn chỉ định rõ ràng một người chơi vào bang hội, ví dụ: khi hai người chơi gặp nhau và một trong số họ là thành viên bang hội.

## Hành động phía client

Những người chơi thuộc bang hội được hiển thị là người chơi của bang hội. Nếu ứng dụng khách trò chơi chưa gặp người chơi của bang hội này, nó sẽ gửi một yêu cầu khác để lấy thông tin bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x65 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | Số lượng người chơi |
| 5 | GuildMemberRelation.Length * PlayerCount | Array of GuildMemberRelation |  | Thành viên |

### Cấu trúc GuildMemberRelation
Mối quan hệ giữa bang hội và thành viên.

Độ dài: 12 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | Id bang hội |
| 4 | 1 | GuildMemberRole |  | Vai trò |
| 7 << 7 | 1 bit | Boolean |  | IsPlayerXuất hiệnMới |
| 7 | 2 | ShortBigEndian |  | Id người chơi |

### Enum GuildMemberRole
Xác định vai trò của thành viên bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | Thành viên là thành viên bình thường không có quyền đặc biệt. |
| 32 | BattleMaster | Thành viên là bậc thầy chiến đấu. |
| 64 | AssistantMaster | Thành viên là trợ lý chủ hội. |
| 128 | GuildMaster | Thành viên là chủ guild. |
| 255 | Undefined | Nhân vật không phải là thành viên nên vai trò không được xác định. |