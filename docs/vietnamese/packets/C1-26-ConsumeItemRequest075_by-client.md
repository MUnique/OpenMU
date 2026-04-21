# C1 26 - ConsumeItemRequest075 (client gửi)

## Được gửi khi nào

Người chơi yêu cầu “tiêu thụ” một item. Có thể là thuốc hồi một loại chỉ số, hoặc jewel để nâng cấp item đích.

## Hành động phía server

Server cố gắng “tiêu thụ” item được chỉ định và trả response tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x26 | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ItemSlot; Chỉ số ô inventory của item sẽ bị tiêu thụ. |
| 4 | 1 | Byte |  | TargetSlot; Nếu item có hiệu ứng lên item khác, ví dụ nâng cấp, trường này chứa chỉ số ô inventory của item đích. |
