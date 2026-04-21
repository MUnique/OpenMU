# C1 A0 - LegacyQuestStateList (server gửi)

## Được gửi khi nào

Sau khi người chơi vào game với nhân vật của mình.

## Hành động phía client

Ứng dụng khách trò chơi sẽ cập nhật trạng thái nhiệm vụ cho hộp thoại nhiệm vụ tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA0 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 6 | nhiệm vụ đếm |
| 4 | LegacyQuestState.Length * | Array of LegacyQuestState |  | tiểu bang |
| 4 << 0 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | CuộnCủaHoàng ĐếNhà Nước |
| 4 << 2 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | Ba Kho Báu CủaMuNhà Nước |
| 4 << 4 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | Đạt đượcTình trạngAnh hùngTrạng thái |
| 4 << 6 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | Bí MậtTốiĐáNhà Nước |
| 5 << 0 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | Giấy chứng nhậnSức mạnhBang |
| 5 << 2 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | Sự Xâm Nhập Của Doanh TrạiNhà Nước |
| 5 << 4 | 2 bit | LegacyQuestState | LegacyQuestState.Inactive | Xâm NhậpNơi Trú ẨnNhà Nước |
| 5 << 6 | 2 bit | LegacyQuestState | LegacyQuestState.Undefined | Trạng thái Quest chưa sử dụng |