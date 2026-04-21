# C1 F3 00 - CharacterList (server gửi)

## Được gửi khi nào

Sau khi game client yêu cầu danh sách nhân vật, thường là ngay sau khi login
thành công.

## Hành động phía client

Game client hiển thị các nhân vật khả dụng của account.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x00 | Packet header - sub packet type identifier |
| 4 | 1 | CharacterCreationUnlockFlags |  | UnlockFlags |
| 5 | 1 | Byte |  | MoveCnt |
| 6 | 1 | Byte |  | CharacterCount |
| 7 | 1 | Boolean |  | IsVaultExtended |
| 8 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### Cấu trúc CharacterData

Dữ liệu của một nhân vật trong danh sách.

Độ dài: 34 bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | SlotIndex |
| 1 | 10 | String |  | Name |
| 12 | 2 | ShortLittleEndian |  | Level |
| 14 | 4 bit | CharacterStatus |  | Status |
| 14 << 4 | 1 bit | Boolean |  | IsItemBlockActive |
| 15 | 18 | Binary |  | Appearance |
| 33 | 1 | GuildMemberRole |  | GuildPosition |

### Enum CharacterStatus

Trạng thái của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Trạng thái bình thường. |
| 1 | Banned | Nhân vật bị cấm chơi. |
| 32 | GameMaster | Nhân vật là game master. |

### Enum GuildMemberRole

Vai trò của thành viên guild.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalMember | Thành viên thường, không có quyền đặc biệt. |
| 32 | BattleMaster | Thành viên là battle master. |
| 64 | AssistantMaster | Thành viên là phó bang chủ. |
| 128 | GuildMaster | Thành viên là bang chủ. |
| 255 | Undefined | Nhân vật không thuộc guild nên vai trò không xác định. |

### Enum CharacterCreationUnlockFlags

Các cờ mở khóa class khi tạo nhân vật mới.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | Không mở khóa class nào. |
| 1 | Summoner | Mở khóa class Summoner. |
| 2 | DarkLord | Mở khóa class Dark Lord. |
| 4 | MagicGladiator | Mở khóa class Magic Gladiator. |
| 8 | RageFighter | Mở khóa class Rage Fighter. |
