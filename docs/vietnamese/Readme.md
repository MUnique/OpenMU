# Tài liệu tiếng Việt (OpenMU)

Thư mục này chứa bản dịch tiếng Việt cho tài liệu trong `docs`.

## Tài liệu đã dịch

- [Tổng quan tài liệu OpenMU](Readme-OpenMU.md)
- [Hệ thống Master Skill](MasterSystem.md)
- [Cơ chế GameMap](GameMap.md)
- [Triển khai kiểm thử cấu trúc packet](PacketStructureTests.md)
- [Tiến độ tính năng dự án](Progress.md)
- [Packet docs tiếng Việt - Đợt 1](packets/Readme.md)
- [Hướng dẫn setup và chạy OpenMU](Setup-Run-Project.md)

## Tài liệu kế hoạch/checklist (đã có tiếng Việt sẵn)

- [Implementation Plan](../ImplementationPlan-PrivateMU.md)
- [Checklist Server](../Checklist-Server.md)
- [Checklist Client](../Checklist-Client.md)
- [Checklist DevOps](../Checklist-DevOps.md)
- [Checklist Project Lead](../Checklist-ProjectLead.md)
- [Mac Server Dev & Debug Guide](../Mac-Server-Dev-Debug-Guide.md)
- [Deploy VPS Checklist](../Deploy-VPS-Checklist.md)
- [Server Execution Log](../ServerExecutionLog.md)

## Trạng thái dịch packet docs

Thư mục `docs/Packets` có số lượng tài liệu rất lớn (hàng trăm file). Các đợt
đầu dịch tay nhóm cốt lõi; phần còn lại đã được sinh bằng script
[`scripts/translate_missing_packet_docs.py`](../../scripts/translate_missing_packet_docs.py)
(machine translation qua Google) rồi chuẩn hoá link `[Packet type]`. Mọi file
`docs/Packets/*.md` hiện đều có bản tương ứng trong `docs/vietnamese/packets/`.

- [x] Đợt 1: Connect/Login/Character/Movement (đã bắt đầu trong `packets/`)
- [x] Đợt 2: Combat/Skill/Inventory/Trade (đã dịch nhóm cốt lõi)
- [x] Đợt 3: Guild/Party/Event/CashShop (đã dịch nhóm cốt lõi)
- [x] Đợt 4: Messenger/Duel/IllusionTemple (đã dịch nhóm cốt lõi)
- [x] Đợt 5: Quest/CashShop (đã dịch nhóm cốt lõi)
- [x] Đợt 6: Login/Character mở rộng (đã dịch nhóm cốt lõi)
- [x] Đợt 7: packet biến thể 075/095 (đã dịch nhóm cốt lõi; xem `packets/Readme.md`)
- [x] Đợt 8: Combat/Skill — 075/095 (đã dịch nhóm cốt lõi; xem `packets/Readme.md`)
- [x] Đợt 9: 095 — skill/hồi sinh/scope (đã dịch nhóm cốt lõi; xem `packets/Readme.md`)
- [x] Đợt 10: 075 — scope/map/skill list (đã dịch nhóm cốt lõi; xem `packets/Readme.md`)
- [x] Đợt 11: 075 — party/guild/item/skill (đã dịch nhóm cốt lõi; xem `packets/Readme.md`)
- [x] **Hoàn tất độ phủ:** 460/460 file `docs/Packets/*.md` đã có bản trong `docs/vietnamese/packets/` (phần lớn đợt cuối: dịch tự động; rà soát thuật ngữ game khi cần)
