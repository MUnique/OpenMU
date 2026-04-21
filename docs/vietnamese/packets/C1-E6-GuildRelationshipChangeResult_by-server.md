# C1 E6 - GuildRelationshipChangeResult (server gửi)

## Được gửi khi nào

Kết quả của yêu cầu thay đổi mối quan hệ bang hội (liên minh hoặc thù địch) được gửi lại cho người yêu cầu.

## Hành động phía client

Người yêu cầu nhìn thấy kết quả của sự thay đổi mối quan hệ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xE6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | GuildRelationshipType |  | Loại mối quan hệ |
| 4 | 1 | GuildRelationshipRequestType |  | Loại yêu cầu |
| 5 | 1 | GuildRelationshipChangeResultType |  | Kết quả |
| 6 | 2 | ShortBigEndian |  | Id GuildMaster |

### Enum GuildRelationshipChangeResultType
Xác định kết quả của yêu cầu thay đổi mối quan hệ bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | Yêu cầu không thành công. |
| 1 | Success | Yêu cầu đã thành công. |
| 16 | GuildNotFound | GUILD_ANS_NOTEXIST_GUILD: Bang hội không tồn tại. |
| 16 | FailedDuringCastleSiege | GUILD_ANS_UNIONFAIL_BY_CASTLE: Chức năng liên minh sẽ bị hạn chế do Cuộc vây hãm lâu đài. |
| 17 | NoAuthorization | GUILD_ANS_NOTEXIST_PERMISSION: Không có quyền thực hiện hành động này. |
| 18 | NotExistExtraStatus | GUILD_ANS_NOTEXIST_EXTRA_STATUS: Trạng thái bổ sung không tồn tại. |
| 19 | NotExistExtraType | GUILD_ANS_NOTEXIST_EXTRA_TYPE: Loại bổ sung không tồn tại. |
| 21 | AlreadyInAlliance | GUILD_ANS_EXIST_RELATIONSHIP_UNION: Bang hội đã có mối quan hệ liên minh. |
| 22 | AlreadyInHostility | GUILD_ANS_EXIST_RELATIONSHIP_RIVAL: Bang hội đã có mối quan hệ thù địch. |
| 23 | GuildAllianceExists | GUILD_ANS_EXIST_UNION: Liên minh bang hội đã tồn tại. |
| 24 | HostileGuildExists | GUILD_ANS_EXIST_RIVAL: Bang hội thù địch đã tồn tại. |
| 25 | GuildAllianceDoesNotExist | GUILD_ANS_NOTEXIST_UNION: Liên minh bang hội không tồn tại. |
| 26 | HostileGuildDoesNotExist | GUILD_ANS_NOTEXIST_RIVAL: Bang hội thù địch không tồn tại. |
| 27 | NotMasterOfGuildAlliance | GUILD_ANS_NOT_UNION_MASTER: Người chơi không phải là chủ nhân của liên minh bang hội. |
| 28 | NotGuildRival | GUILD_ANS_NOT_GUILD_RIVAL: Bang hội không phải là bang hội đối thủ. |
| 29 | IncompleteRequirementsToCreateAlliance | GUILD_ANS_CANNOT_BE_UNION_MASTER_GUILD: Các yêu cầu để tạo liên minh chưa đầy đủ. |
| 30 | MaximumNumberOfGuildsInAllianceReached | GUILD_ANS_EXCEED_MAX_UNION_MEMBER: Đã đạt đến số lượng bang hội tối đa trong liên minh. |
| 32 | RequestCancelled | GUILD_ANS_CANCEL_REQUEST: Yêu cầu đã bị hủy. |
| 161 | AllianceMasterNotInGens | GUILD_ANS_UNION_MASTER_NOT_GENS: Chủ liên minh không thuộc Gens. |
| 162 | GuildMasterNotInGens | GUILD_ANS_GUILD_MASTER_NOT_GENS: Chủ bang hội không thuộc Gens. |
| 163 | DifferentGens | GUILD_ANS_UNION_MASTER_DISAGREE_GENS: Chủ liên minh và chủ bang hội thuộc các Gen khác nhau. |

### Enum GuildRelationshipType
Mô tả loại mối quan hệ giữa các bang hội.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại mối quan hệ không xác định. |
| 1 | Alliance | Kiểu quan hệ liên minh. |
| 2 | Hostility | Kiểu quan hệ thù địch. |

### Enum GuildRelationshipRequestType
Mô tả loại yêu cầu.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại yêu cầu không xác định. |
| 1 | Join | Kiểu tham gia. |
| 2 | Leave | Kiểu nghỉ phép. |