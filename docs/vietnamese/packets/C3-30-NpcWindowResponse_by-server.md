# C3 30 - NpcWindowResponse (server gửi)

## Được gửi khi nào

Sau khi máy khách nói chuyện với NPC, điều này sẽ khiến hộp thoại mở ra ở phía máy khách.

## Hành động phía client

Máy khách mở hộp thoại được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 11 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x30 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | NpcWindow |  | Cửa sổ |

### Enum NpcWindow
Xác định loại cửa sổ npc sẽ được hiển thị trên máy khách.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Merchant | Một cửa sổ thương mại. |
| 1 | Merchant1 | Một cửa sổ thương mại khác. |
| 2 | VaultStorage | Một kho lưu trữ. |
| 3 | ChaosMachine | Một cửa sổ máy hỗn loạn. |
| 4 | DevilSquare | Một cửa sổ hình vuông quỷ dữ. |
| 6 | BloodCastle | Một cửa sổ lâu đài máu. |
| 7 | PetTrainer | Cửa sổ huấn luyện thú cưng. |
| 9 | Lahap | Cửa sổ lahap. |
| 12 | CastleSeniorNPC | Cửa sổ cao cấp của lâu đài. |
| 17 | ElphisRefinery | Cửa sổ nhà máy lọc dầu elphis. |
| 18 | RefineStoneMaking | Cửa sổ làm đá tinh luyện. |
| 19 | RemoveJohOption | Cửa sổ loại bỏ tùy chọn viên ngọc quý của sự hòa hợp. |
| 20 | IllusionTemple | Cửa sổ ngôi đền ảo ảnh. |
| 21 | ChaosCardCombination | Cửa sổ kết hợp thẻ hỗn loạn. |
| 22 | CherryBlossomBranchesAssembly | Cửa sổ lắp ráp cành hoa anh đào. |
| 23 | SeedMaster | Cửa sổ chính của hạt giống. |
| 24 | SeedResearcher | Cửa sổ nhà nghiên cứu hạt giống. |
| 25 | StatReInitializer | Cửa sổ khởi tạo lại stat. |
| 32 | DelgadoLuckyCoinRegistration | Cửa sổ đăng ký đồng xu may mắn delgado. |
| 33 | DoorkeeperTitusDuelWatch | Người giữ cửa Titus đấu tay đôi cửa sổ. |
| 35 | LugardDoppelgangerEntry | Cửa sổ nhập cảnh doppelganger lugard. |
| 36 | JerintGaionEvententry | Cửa sổ nhập sự kiện jerint gaion. |
| 37 | JuliaWarpMarketServer | Cửa sổ máy chủ thị trường Julia Warp. |
| 38 | CombineLuckyItem | Cửa sổ hộp thoại cho phép trao đổi hoặc tinh chế Vật phẩm may mắn. Được sử dụng bởi NPC "David". |