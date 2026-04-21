# C1 F6 0C - QuestProgress (server gửi)

## Được gửi khi nào

Đầu tiên, sau khi ứng dụng trò chơi yêu cầu khởi tạo một nhiệm vụ và nhiệm vụ đó đã được kích hoạt. Thứ hai, sau khi khách hàng trò chơi yêu cầu bước nhiệm vụ tiếp theo.

## Hành động phía client

Khách hàng hiển thị tiến trình nhiệm vụ tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 251 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0C | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số nhiệm vụ |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |
| 8 | 1 | Byte |  | Tình trạngĐếm |
| 9 | 1 | Byte |  | Số phần thưởng |
| 11 | QuestCondition.Length * ConditionCount | Array of QuestCondition |  | Điều kiện |
| 141 | QuestReward.Length * RewardCount | Array of QuestReward |  | Phần thưởng |

### Cấu trúc QuestCondition
Xác định một điều kiện phải được đáp ứng để hoàn thành nhiệm vụ.

Độ dài: 26 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | ConditionType |  | Type |
| 4 | 2 | ShortLittleEndian |  | Id yêu cầu; Tùy thuộc vào loại điều kiện, trường này chứa mã định danh của thứ được yêu cầu, ví dụ: Số Quái vật, Id Vật phẩm, Cấp độ. |
| 6 | 4 | IntegerLittleEndian |  | Số lượng bắt buộc |
| 10 | 4 | IntegerLittleEndian |  | Số lượng hiện tại |
| 14 | 12 | Binary |  | Dữ liệu bắt buộc; Nếu loại điều kiện là 'Mục' thì trường này chứa dữ liệu mục, ngoại trừ id mục. Id mục có thể được tìm thấy trong trường RequirementId. |

### Enum ConditionType
Xác định loại điều kiện.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | Không có điều kiện được xác định. |
| 1 | MonsterKills | Điều kiện là tiêu diệt một lượng quái vật nhất định. |
| 2 | Skill | Điều kiện là phải học một kỹ năng cụ thể. |
| 4 | Item | Điều kiện là tìm một mặt hàng được chỉ định và có nó trong kho. |
| 8 | Level | Điều kiện là đạt đến cấp độ ký tự được chỉ định. |
| 16 | ClientAction | Điều kiện là một hành động của khách hàng. Ví dụ: đây có thể là phần hoàn thành của một hướng dẫn. |
| 32 | RequestBuff | Điều kiện là yêu cầu một NPC hỗ trợ. |
| 64 | EventMapPlayerKills | Điều kiện là giết một lượng người chơi cụ thể trong một sự kiện. |
| 65 | EventMapMonsterKills | Điều kiện là tiêu diệt một lượng quái vật cụ thể trong một sự kiện. |
| 66 | BloodCastleGate | Điều kiện là phải phá hủy cổng lâu đài máu. |
| 256 | WinBloodCastle | Điều kiện là phải thắng sự kiện lâu đài máu. |
| 257 | WinChaosCastle | Điều kiện là phải chiến thắng lâu đài hỗn loạn. |
| 258 | WinDevilSquare | Điều kiện là phải thắng sự kiện quảng trường ma quỷ. |
| 259 | WinIllusionTemple | Điều kiện là phải chiến thắng sự kiện đền ảo ảnh. |
| 260 | DevilSquarePoints | Điều kiện là đạt được một số điểm cụ thể trong sự kiện quảng trường ma quỷ. |
| 261 | Money | Điều kiện là đưa ra một lượng zen cụ thể. |
| 262 | PvpPoints | Điều kiện là phải đạt được số điểm PVP nhất định. |
| 263 | NpcTalk | Điều kiện là nói chuyện với một NPC cụ thể. |

### Cấu trúc QuestReward
Xác định phần thưởng được trao khi nhiệm vụ hoàn thành.

Độ dài: 22 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | RewardType |  | Type |
| 4 | 2 | ShortLittleEndian |  | Id phần thưởng; Tùy thuộc vào loại điều kiện, trường này chứa mã định danh của thứ được yêu cầu, ví dụ: Số Quái vật, Id Vật phẩm, Cấp độ. |
| 6 | 4 | IntegerLittleEndian |  | Số phần thưởng |
| 10 | 12 | Binary |  | Phần thưởngItemData; Nếu loại phần thưởng là 'Vật phẩm' thì trường này chứa dữ liệu vật phẩm của nó. |

### Enum RewardType
Xác định phần thưởng được trao khi nhiệm vụ hoàn thành.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | Không có phần thưởng được xác định. |
| 1 | Experience | Phần thưởng là thêm kinh nghiệm cho nhân vật. |
| 2 | Money | Phần thưởng được thêm tiền vào kho. |
| 4 | Item | Phần thưởng là một vật phẩm được thêm vào kho. |
| 16 | GensContribution | Phần thưởng là điểm đóng góp của thị tộc. |
| 32 | Random | Phần thưởng là ngẫu nhiên?. |