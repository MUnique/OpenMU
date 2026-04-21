# C3 19 - TargetedSkill (client gửi)

## Được gửi khi nào

Người chơi thực hiện một kỹ năng với mục tiêu, ví dụ: tấn công hoặc buff.

## Hành động phía server

Sát thương được tính toán và mục tiêu sẽ bị bắn trúng nếu cuộc tấn công thành công. Một phản hồi sẽ được gửi lại kèm theo sát thương gây ra và tất cả những người chơi xung quanh sẽ nhận được thông báo hoạt hình.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x19 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id mục tiêu |