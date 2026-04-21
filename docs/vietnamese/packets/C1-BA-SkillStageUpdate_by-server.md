# C1 BA - SkillStageUpdate (server gửi)

## Được gửi khi nào

Sau khi người chơi bắt đầu một kỹ năng cần được tải lên, chẳng hạn như Nova.

## Hành động phía client

Khách hàng có thể hiển thị cường độ tải.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBA | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortLittleEndian |  | ID đối tượng |
| 5 | 1 | Byte | 0x28 | Số kỹ năng |
| 6 | 1 | Byte |  | Sân khấu |