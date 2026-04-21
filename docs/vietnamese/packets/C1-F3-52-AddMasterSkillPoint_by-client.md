# C1 F3 52 - AddMasterSkillPoint (client gửi)

## Được gửi khi nào

Người chơi muốn thêm hoặc tăng cấp độ của một kỹ năng bậc thầy cụ thể trong cây kỹ năng bậc thầy.

## Hành động phía server

Thêm hoặc tăng cấp độ kỹ năng chủ của kỹ năng được chỉ định, nếu nhân vật được phép làm điều đó. Một phản hồi sẽ được gửi lại cho khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x52 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | ID kỹ năng |