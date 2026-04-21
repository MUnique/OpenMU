# C1 D0 08 - EnterOnGatekeeperRequest (client gửi)

## Được gửi khi nào

Một người chơi đang thực hiện nhiệm vụ "Vào 'Khu vực bóng tối'" (số 6), nói chuyện với NPC người gác cổng trong 'Doanh trại Balgass'.

## Hành động phía server

Nó sẽ đưa người chơi đến bản đồ 'Nơi trú ẩn Balgass', nơi những quái vật cần thiết phải bị tiêu diệt để tiếp tục nhiệm vụ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD0 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x08 | Tiêu đề gói - mã định danh loại gói phụ |