# C1 01 - ObjectMessage (server gửi)

## Được gửi khi nào

Máy chủ muốn hiển thị thông báo phía trên bất kỳ loại ký tự nào, kể cả NPC.

## Hành động phía client

Thông báo được hiển thị phía trên ký tự.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | ID đối tượng |
| 5 |  | String |  | Tin nhắn |