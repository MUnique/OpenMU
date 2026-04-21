# C1 11 - ObjectHitExtended (server gửi)

## Được gửi khi nào

Một vật bị đánh trong hai trường hợp: 1. Khi người chơi của mình bị đánh; 2. Khi người chơi của mình tấn công một vật thể khác và bị trúng đòn.

## Hành động phía client

Sát thương được thể hiện ở đối tượng nhận đòn đánh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x11 | Tiêu đề gói - mã định danh loại gói |
| 3 << 0 | 4 bit | DamageKind |  | Loại |
| 3 << 4 | 1 bit | Boolean |  | IsRageMáy bay chiến đấuStreakHit |
| 3 << 5 | 1 bit | Boolean |  | IsRageFighterStreakFinalHit |
| 3 << 6 | 1 bit | Boolean |  | Là sát thương gấp đôi |
| 3 << 7 | 1 bit | Boolean |  | là sát thương gấp ba lần |
| 4 | 2 | ShortLittleEndian |  | ID đối tượng |
| 6 | 1 | Byte |  | Tình trạng sức khỏe; Nhận hoặc đặt trạng thái của lượng máu còn lại theo phân số 1/250. |
| 7 | 1 | Byte |  | Trạng thái lá chắn; Nhận hoặc đặt trạng thái của lá chắn còn lại theo phân số 1/250. |
| 8 | 4 | IntegerLittleEndian |  | Sức khỏeSát thương |
| 12 | 4 | IntegerLittleEndian |  | KhiênSát thương |

### Enum DamageKind
Xác định loại sát thương.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NormalRed | Màu đỏ, dùng khi bị sát thương thông thường. |
| 1 | IgnoreDefenseCyan | Màu lục lam, thường được sử dụng để bỏ qua sát thương phòng thủ. |
| 2 | ExcellentLightGreen | Màu xanh nhạt, thường được sử dụng bởi khả năng sát thương tuyệt vời. |
| 3 | CriticalBlue | Màu xanh lam, thường được sử dụng bởi sát thương chí mạng. |
| 4 | ReflectedLightPink | Màu hồng nhạt, thường được sử dụng bởi phản xạ sát thương. |
| 5 | PoisonDarkGreen | Màu xanh đậm, thường dùng do chất độc gây sát thương. |
| 6 | DarkPink | Màu hồng đậm. |
| 7 | White | Màu trắng. |