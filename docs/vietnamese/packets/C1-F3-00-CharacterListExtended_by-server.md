# C1 F3 00 - CharacterListExtended (server gửi)

## Được gửi khi nào

Sau khi ứng dụng khách trò chơi yêu cầu, thường là sau khi đăng nhập thành công.

## Hành động phía client

Ứng dụng khách trò chơi hiển thị các ký tự có sẵn của tài khoản.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | CharacterCreationUnlockFlags |  | Mở khóaCờ |
| 5 | 1 | Byte |  | Di chuyểnCnt |
| 6 | 1 | Byte |  | Số ký tự |
| 7 | 1 | Boolean |  | IsVaultExtends |
| 8 | CharacterData.Length * CharacterCount | Array of CharacterData |  | nhân vật |

### Cấu trúc CharacterData
Dữ liệu của một ký tự trong danh sách.

Độ dài: 44 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Chỉ số vị trí |
| 1 | 10 | String |  | Name |
| 12 | 2 | ShortLittleEndian |  | Mức độ |
| 14 | 4 bit | CharacterStatus |  | Trạng thái |
| 14 << 4 | 1 bit | Boolean |  | IsItemBlockHoạt động |
| 15 | 27 | Binary |  | Vẻ bề ngoài |
| 42 | 1 | GuildMemberRole |  | Bang hộiVị trí |

### Enum CharacterStatus
Trạng thái của một nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Trạng thái của nhân vật là bình thường. |
| 1 | Banned | Nhân vật bị cấm khỏi trò chơi. |
| 32 | GameMaster | Nhân vật là một bậc thầy trò chơi. |

### Enum GuildMemberRole
Xác định vai trò của thành viên bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | Thành viên là thành viên bình thường không có quyền đặc biệt. |
| 32 | BattleMaster | Thành viên là bậc thầy chiến đấu. |
| 64 | AssistantMaster | Thành viên là trợ lý chủ hội. |
| 128 | GuildMaster | Thành viên là chủ guild. |
| 255 | Undefined | Nhân vật không phải là thành viên nên vai trò không được xác định. |

### Enum CharacterCreationUnlockFlags
Cờ mở khóa các lớp ký tự được chỉ định để tạo ký tự mới.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | Không có lớp mở khóa. |
| 1 | Summoner | Mở khóa lớp người triệu hồi. |
| 2 | DarkLord | Mở khóa lớp chúa tể bóng tối. |
| 4 | MagicGladiator | Mở khóa lớp đấu sĩ ma thuật. |
| 8 | RageFighter | Mở khóa lớp chiến binh cuồng nộ. |