# C1 91 - MiniGameOpeningState (server gửi)

## Được gửi khi nào

Người chơi yêu cầu biết trạng thái mở đầu hiện tại của một sự kiện trò chơi nhỏ bằng cách nhấp vào một mục vé.

## Hành động phía client

Trạng thái mở đầu của sự kiện (thời gian nhập còn lại, v.v.) được hiển thị trên máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x91 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | MiniGameType |  | Loại trò chơi |
| 4 | 1 | Byte |  | Còn lạiNhập thời gianPhút |
| 5 | 1 | Byte |  | Số lượng người dùng |
| 6 | 1 | Byte |  | Còn lạiNhậpThời gianPhútThấp; Chỉ dùng cho Chaos Castle. Trong trường hợp này, trường này chứa byte thấp hơn của số phút còn lại. Đối với các loại sự kiện khác, trường này không được sử dụng. |

### Enum MiniGameType
Xác định loại trò chơi nhỏ.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại trò chơi nhỏ không xác định. |
| 1 | DevilSquare | Trò chơi mini hình vuông ma quỷ. |
| 2 | BloodCastle | Game mini lâu đài máu. |
| 3 | CursedTemple | Mini game ngôi đền bị nguyền rủa |
| 4 | ChaosCastle | Game mini lâu đài hỗn loạn. |
| 5 | IllusionTemple | Game mini ngôi đền ảo ảnh. |
| 6 | Doppelganger | Trò chơi mini doppelgänger. |