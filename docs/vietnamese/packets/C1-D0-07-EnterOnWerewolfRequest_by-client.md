# C1 D0 07 - EnterOnWerewolfRequest (client gửi)

## Được gửi khi nào

Một người chơi đang thực hiện nhiệm vụ "Xâm nhập Doanh trại Balgass" (số 5), nói chuyện với npc Người sói trong Crywolf.

## Hành động phía server

Nó sẽ đưa người chơi đến bản đồ 'Doanh trại Balgass', nơi những quái vật cần thiết phải bị tiêu diệt để tiếp tục nhiệm vụ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD0 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |