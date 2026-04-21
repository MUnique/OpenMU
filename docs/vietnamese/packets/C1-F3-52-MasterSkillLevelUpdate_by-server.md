# C1 F3 52 - MasterSkillLevelUpdate (server gửi)

## Được gửi khi nào

Sau khi cấp độ kỹ năng bậc thầy đã được thay đổi (thường là tăng lên).

## Hành động phía client

Cấp độ được cập nhật trong cây kỹ năng chính.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 28 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x52 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Thành công |
| 6 | 2 | ShortLittleEndian |  | MasterLevelUpPoints |
| 8 | 1 | Byte |  | MasterSkillIndex; Chỉ số kỹ năng bậc thầy trên cây kỹ năng bậc thầy của khách hàng đối với lớp nhân vật nhất định. |
| 12 | 2 | ShortLittleEndian |  | Bậc ThầyKỹ NăngSố |
| 16 | 1 | Byte |  | Mức độ |
| 20 | 4 | Float |  | Giá trị hiển thị |
| 24 | 4 | Float |  | DisplayValueOfNextLevel |