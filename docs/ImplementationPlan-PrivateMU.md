# Private MU Implementation Plan (PC + Mobile + Web)

Mục tiêu: xây dựng server và client cho game MU Online private, ưu tiên chạy local ổn định trước, sau đó mở rộng cross-platform (PC/Mobile) và cuối cùng tích hợp web.

## 1) Scope & Direction

- [ ] Chốt hướng kỹ thuật tổng thể:
  - [ ] Server: fork từ `OpenMU`.
  - [ ] Client giai đoạn đầu: protocol Season 6.
  - [ ] Client cross-platform:
    - [ ] Lựa chọn A: `bernatvadell/muonline` (MonoGame + .NET, mobile support mạnh).
    - [ ] Lựa chọn B: `afrokick/UniMU` (Unity thuần, để mở rộng WebGL sau này).
- [ ] Chốt MVP gameplay (vertical slice):
  - [ ] Login -> tạo/chọn nhân vật -> vào map -> di chuyển -> đánh quái -> nhặt đồ.
- [ ] Định nghĩa tiêu chí "Done" cho MVP:
  - [ ] Chơi ổn định 30-60 phút không crash.
  - [ ] Không có packet error blocker.
  - [ ] Có log để debug được sự cố kết nối/combat.

## 2) Server Foundation (OpenMU Fork)

- [ ] Fork `https://github.com/MUnique/OpenMU`.
- [ ] Tạo branch strategy:
  - [ ] `main` (stable), `develop` (integration), `feature/*`.
- [ ] Thực hiện theo QuickStart:
  - [ ] `https://github.com/MUnique/OpenMU/blob/master/QuickStart.md`
- [ ] Khởi động local stack thành công:
  - [ ] Database.
  - [ ] ConnectServer/LoginServer/GameServer.
- [ ] Xác nhận flow local cơ bản:
  - [ ] Tạo account.
  - [ ] Đăng nhập thành công.
  - [ ] Vào game map thành công.
- [ ] Ghi lại runbook local trong docs:
  - [ ] Lệnh setup.
  - [ ] Lệnh run.
  - [ ] Các lỗi thường gặp và cách xử lý.

## 3) Protocol & Networking (Season 6 First)

- [ ] Chốt 1 profile protocol Season 6 làm baseline.
- [ ] Tạo danh sách packet critical cho MVP:
  - [ ] Login/Auth.
  - [ ] Character list/select.
  - [ ] Enter world/map.
  - [ ] Move/rotation.
  - [ ] Attack/skill.
  - [ ] Inventory/pickup.
- [ ] Thêm packet trace/log (dev mode):
  - [ ] Log packet inbound/outbound theo opcode.
  - [ ] Có flag bật/tắt log để tránh noisy.
- [ ] Tạo checklist mismatch packet:
  - [ ] Packet nào fail.
  - [ ] Nguyên nhân.
  - [ ] Cách fix.

## 4) Client Track (Unity/MonoGame)

### 4.1 Chọn hướng client

- [ ] Đánh giá theo tiêu chí:
  - [ ] Tốc độ ra MVP.
  - [ ] Mức độ dễ kết nối OpenMU.
  - [ ] Chi phí maintain dài hạn.
  - [ ] Mức độ sẵn sàng cho Mobile + Web.
- [ ] Chốt 1 hướng chính và 1 hướng dự phòng.

### 4.2 Nếu đi Unity (ưu tiên cho Web sau này)

- [ ] Khởi tạo project Unity version team đã chốt.
- [ ] Tích hợp Unity Input System.
- [ ] Tích hợp joystick package miễn phí cho mobile.
- [ ] Tách input layer:
  - [ ] Desktop input (keyboard/mouse).
  - [ ] Mobile input (touch/joystick).
  - [ ] Cùng sử dụng một gameplay layer.
- [ ] Kết nối protocol Season 6 tới OpenMU local.

### 4.3 Nếu đi MonoGame (ưu tiên mobile sớm)

- [ ] Setup và build được client mẫu `bernatvadell/muonline`.
- [ ] Kết nối OpenMU local qua packet baseline.
- [ ] Chốt roadmap polish UX cho PC và mobile.

## 5) Mobile Optimization

- [ ] Xác định thiết bị test tối thiểu (Android yếu/trung bình, iOS nếu có).
- [ ] Giảm GC spike:
  - [ ] Object pooling cho entity/projectile/effect.
  - [ ] Hạn chế cấp phát mới trong update loop.
- [ ] Tối ưu rendering:
  - [ ] Giảm overdraw.
  - [ ] Bật/tắt quality profile theo thiết bị.
- [ ] Tối ưu network:
  - [ ] Debounce/rate-limit input packet không cần thiết.
  - [ ] Kiểm soát tần suất update object.
- [ ] KPI mobile MVP:
  - [ ] FPS trung bình đạt mục tiêu.
  - [ ] Nhiệt độ máy/hao pin chấp nhận được.
  - [ ] Ping/latency hiển thị ổn định trong gameplay cơ bản.

## 6) Security, Reliability, Observability

- [ ] Server-authoritative cho movement/combat/drop.
- [ ] Validate input packet ở server:
  - [ ] State transition hợp lệ.
  - [ ] Rate-limit packet bất thường.
- [ ] Bảo mật account:
  - [ ] Không hardcode secret trong client.
  - [ ] Quản lý secret theo môi trường.
- [ ] Logging/metrics căn bản:
  - [ ] Online player count.
  - [ ] Packet error rate.
  - [ ] Disconnect reason.
  - [ ] Average RTT.
- [ ] Có cảnh báo cho sự cố nghiêm trọng:
  - [ ] Login fail bất thường.
  - [ ] Crash loop.
  - [ ] DB connection issue.

## 7) Web Platform (Future Phase)

- [ ] Làm POC Unity WebGL sớm để đánh giá rủi ro:
  - [ ] Memory footprint.
  - [ ] Input UX trên browser.
  - [ ] Network overhead qua WebSocket/HTTP bridge (nếu cần).
- [ ] Đánh giá go/no-go dựa trên metric thật:
  - [ ] FPS.
  - [ ] Load time.
  - [ ] Độ ổn định kết nối.
- [ ] Nếu WebGL chưa đạt:
  - [ ] Ưu tiên website cho account, ranking, tin tức, launcher.
  - [ ] Giữ gameplay trên native PC/Mobile.

## 8) Release Plan & Milestones

- [ ] Milestone 1 - Local Boot:
  - [ ] OpenMU chạy local thành công.
  - [ ] Login + vào map được.
- [ ] Milestone 2 - Vertical Slice:
  - [ ] Hoàn thành full flow MVP gameplay.
- [ ] Milestone 3 - Cross-platform Alpha:
  - [ ] Chạy được PC + Android (iOS nếu có).
- [ ] Milestone 4 - Closed Test:
  - [ ] Có observability và bug triage loop.
- [ ] Milestone 5 - Web POC:
  - [ ] Có kết luận kỹ thuật rõ ràng cho web gameplay.

## 9) Weekly Execution Checklist (Mẫu)

- [ ] Week 1:
  - [ ] Fork/OpenMU setup + QuickStart pass.
  - [ ] Document runbook local.
- [ ] Week 2:
  - [ ] Packet baseline Season 6.
  - [ ] Login + character select stable.
- [ ] Week 3:
  - [ ] Enter world + movement stable.
  - [ ] Combat cơ bản.
- [ ] Week 4:
  - [ ] Inventory/pickup + bugfix.
  - [ ] Internal MVP review.
- [ ] Week 5-6:
  - [ ] Mobile input + performance pass 1.
- [ ] Week 7-8:
  - [ ] Reliability/security hardening + closed test prep.

## 10) Open Decisions (Cần chốt sớm)

- [ ] Chọn client track chính: Unity hay MonoGame?
- [ ] Mục tiêu ưu tiên 3 tháng đầu:
  - [ ] "Playable MVP nhanh" hay "Nền tảng Unity cho web về sau"?
- [ ] Team size và phân công:
  - [ ] Ai phụ trách server.
  - [ ] Ai phụ trách client.
  - [ ] Ai phụ trách DevOps/CI.

## 11) Quy ước thuật ngữ (Dùng nhất quán trong team)

- [ ] Thống nhất thuật ngữ Việt/Anh:
  - [ ] MVP = bản khả dụng tối thiểu.
  - [ ] Vertical slice = luồng chơi đầy đủ tối thiểu.
  - [ ] Baseline protocol = bộ giao thức chuẩn ban đầu (Season 6).
  - [ ] Server-authoritative = server quyết định trạng thái cuối cùng.
  - [ ] Observability = khả năng quan sát hệ thống (log/metrics/alert).
  - [ ] Hardening = gia cố ổn định và bảo mật trước khi test rộng.
- [ ] Quy ước văn phong tài liệu:
  - [ ] Viết tiếng Việt có dấu.
  - [ ] Giữ nguyên tên kỹ thuật trong code (ví dụ: `LoginServer`, `GameServer`, `WebGL`).
  - [ ] Mỗi hạng mục có tiêu chí hoàn thành rõ ràng.

## 12) Checklist theo vai trò (Để giao task)

### 12.1 Server Engineer

- [ ] Hoàn thành local server theo QuickStart:
  - [ ] Database + ConnectServer + LoginServer + GameServer chạy ổn định.
  - [ ] Tạo account, đăng nhập và vào map thành công.
- [ ] Chốt protocol Season 6 cho giai đoạn đầu:
  - [ ] Xác định danh sách packet cốt lõi cho MVP.
  - [ ] Có cơ chế log packet inbound/outbound ở môi trường dev.
- [ ] Đảm bảo tính đúng đắn gameplay phía server:
  - [ ] Movement/combat/drop được xử lý server-authoritative.
  - [ ] Validate state transition để chặn packet không hợp lệ.
- [ ] Bổ sung độ tin cậy:
  - [ ] Cơ chế lưu account an toàn theo chu kỳ.
  - [ ] Có xử lý lỗi rõ ràng, không fail im lặng.

### 12.2 Client Engineer (Unity/MonoGame)

- [ ] Thiết lập client track chính:
  - [ ] Chốt Unity hoặc MonoGame cho phase MVP.
  - [ ] Build chạy được trên PC trước, sau đó mở rộng mobile.
- [ ] Kết nối được với OpenMU local:
  - [ ] Hoàn thành login -> chọn nhân vật -> vào map.
  - [ ] Hoàn thành di chuyển -> đánh quái -> nhặt đồ.
- [ ] Tối ưu input đa nền tảng:
  - [ ] Desktop: keyboard/mouse.
  - [ ] Mobile: touch + joystick.
  - [ ] Hai mode dùng chung gameplay layer.
- [ ] Tối ưu hiệu năng mobile:
  - [ ] Giảm cấp phát runtime, ưu tiên object pooling.
  - [ ] Đạt KPI FPS/ping/nhiệt độ thiết bị mục tiêu.

### 12.3 DevOps/Platform Engineer

- [ ] Chuẩn hóa môi trường:
  - [ ] Tách cấu hình `dev/staging/prod`.
  - [ ] Quản lý secret theo môi trường, không để trong client.
- [ ] Thiết lập CI cơ bản:
  - [ ] Build server tự động.
  - [ ] Build client theo target chính.
  - [ ] Chạy smoke test cho luồng đăng nhập.
- [ ] Thiết lập quan sát hệ thống:
  - [ ] Thu thập metrics: online, packet error rate, RTT, disconnect reason.
  - [ ] Thiết lập cảnh báo khi login fail/crash loop/lỗi DB tăng đột biến.
- [ ] Hỗ trợ phát hành nội bộ:
  - [ ] Tạo checklist phát hành cho Closed Test.
  - [ ] Có quy trình rollback khi bản phát hành lỗi.

### 12.4 Product Owner/Project Lead (Khuyến nghị)

- [ ] Chốt mục tiêu 8 tuần đầu:
  - [ ] Ưu tiên ra bản chơi được nhanh hay ưu tiên nền tảng cho Web.
- [ ] Chốt Definition of Done cho từng milestone.
- [ ] Theo dõi burn-down theo tuần:
  - [ ] Task blocker.
  - [ ] Rủi ro kỹ thuật.
  - [ ] Quyết định cần chốt sớm.

## 13) Tài liệu checklist theo vai trò (Tách riêng)

- [ ] Server Engineer:
  - [ ] Xem `Checklist-Server.md`.
- [ ] Client Engineer:
  - [ ] Xem `Checklist-Client.md`.
- [ ] DevOps/Platform Engineer:
  - [ ] Xem `Checklist-DevOps.md`.
- [ ] Project Lead/Product Owner:
  - [ ] Xem `Checklist-ProjectLead.md`.
