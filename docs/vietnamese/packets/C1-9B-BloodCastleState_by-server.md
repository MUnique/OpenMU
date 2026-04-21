# C1 9B - BloodCastleState (server gửi)

## Được gửi khi nào

Tình trạng của sự kiện lâu đài máu sắp thay đổi.

## Hành động phía client

Phía khách hàng hiển thị một thông báo về trạng thái thay đổi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x9B | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Status |  | Tình trạng |
| 4 | 2 | ShortLittleEndian |  | Còn lại thứ hai |
| 6 | 2 | ShortLittleEndian |  | MaxMonster |
| 8 | 2 | ShortLittleEndian |  | CurMonster |
| 10 | 2 | ShortLittleEndian |  | Id chủ sở hữu mặt hàng |
| 12 | 1 | Byte |  | Cấp độ vật phẩm |

### Enum Status
Xác định trạng thái của sự kiện.

| Value | Name | Description |
|-------|------|-------------|
| 0 | BloodCastleStarted | Sự kiện lâu đài máu vừa mới bắt đầu và đang diễn ra. |
| 1 | BloodCastleGateNotDestroyed | Sự kiện lâu đài máu đang diễn ra nhưng cổng không bị phá hủy. |
| 2 | BloodCastleEnded | Sự kiện lâu đài máu đã kết thúc. |
| 4 | BloodCastleGateDestroyed | Sự kiện lâu đài máu đang diễn ra và cánh cổng bị phá hủy. |
| 5 | ChaosCastleStarted | Sự kiện lâu đài hỗn loạn vừa mới bắt đầu và đang diễn ra. |
| 6 | ChaosCastleRunning | Sự kiện lâu đài hỗn loạn đang diễn ra. |
| 7 | ChaosCastleEnded | Sự kiện lâu đài hỗn loạn đã kết thúc. |
| 8 | ChaosCastleStageOne | Sự kiện lâu đài hỗn loạn đã đạt đến giai đoạn thu nhỏ bản đồ đầu tiên. |
| 9 | ChaosCastleStageTwo | Sự kiện lâu đài hỗn loạn đã đạt đến giai đoạn thu nhỏ bản đồ thứ hai. |
| 10 | ChaosCastleStageThree | Sự kiện lâu đài hỗn loạn đã đạt đến giai đoạn thu nhỏ bản đồ thứ ba. |