# Checklist Server Engineer (Private MU)

## 0) Việc cần làm ngay (theo thứ tự ưu tiên)

### 0.1 Chuẩn bị môi trường local (Preflight)

- [ ] Xác nhận công cụ đã sẵn sàng:
  - [ ] `docker --version`
  - [ ] `docker compose version`
  - [ ] `dotnet --version` (>= 10 nếu chạy source)
- [ ] Xác nhận các cổng quan trọng chưa bị chiếm:
  - [ ] `80`, `55901-55906`, `44405-44406`, `55980`
- [ ] Chốt mode chạy cho phase này:
  - [ ] **Docker all-in-one** (ưu tiên để lên nhanh và test end-to-end).
  - [ ] **Run từ source** (khi cần debug sâu server code).

### 0.2 Boot nhanh bằng Docker (khuyến nghị làm trước)

- [ ] Đi vào `deploy/all-in-one`.
- [ ] Chạy local stack:
  - [ ] `docker compose up -d --no-build`
- [ ] Kiểm tra container đã lên:
  - [ ] `docker compose ps`
  - [ ] `docker compose logs -f openmu-startup` (kiểm tra init DB không lỗi)
- [ ] Mở admin panel:
  - [ ] `http://localhost/` (mặc định `admin/openmu`)
- [ ] Tại admin panel:
  - [ ] Start 2 connect servers.
  - [ ] Start ít nhất 1 game server.
  - [ ] Bật Auto Start nếu muốn tự chạy các lần sau.

### 0.3 Xác nhận login/game flow tối thiểu (Milestone 1 gate)

- [ ] Dùng client Season 6 kết nối vào local server.
- [ ] Dùng test account có sẵn (ví dụ `test0/test0`, `testgm/testgm`).
- [ ] Đi qua flow bắt buộc:
  - [ ] Login thành công.
  - [ ] Character select thành công.
  - [ ] Enter map thành công.
- [ ] Nếu bị disconnect sau server select:
  - [ ] Set IP resolve về local/loopback trong admin panel hoặc start parameter.
  - [ ] Dùng loopback IP `127.127.127.127` (không dùng `127.0.0.1` cho game client).

### 0.4 Chuẩn bị nhánh làm việc server

- [ ] Tạo nhánh theo scope:
  - [ ] `feature/server-local-boot`
  - [ ] `feature/server-season6-baseline`
- [ ] Tạo file log tiến độ (khuyến nghị):
  - [ ] `docs/ServerExecutionLog.md` để ghi lỗi, nguyên nhân, cách fix.

## 1) Local Setup & Boot

- [x] Fork `OpenMU` và đồng bộ branch làm việc.
- [ ] Chạy local stack theo QuickStart:
  - [ ] Database.
  - [ ] ConnectServer.
  - [ ] LoginServer.
  - [ ] GameServer.
- [ ] Xác nhận flow cơ bản:
  - [ ] Tạo account.
  - [ ] Đăng nhập thành công.
  - [ ] Vào map thành công.

## 2) Protocol Season 6

- [ ] Chốt baseline protocol Season 6 cho phase đầu.
- [ ] Lập danh sách packet cốt lõi:
  - [ ] Login/Auth.
  - [ ] Character list/select.
  - [ ] Enter world/map.
  - [ ] Move/rotation.
  - [ ] Attack/skill.
  - [ ] Inventory/pickup.
- [ ] Bật log packet inbound/outbound ở môi trường dev.
- [ ] Tạo danh sách mismatch packet và theo dõi đến khi đóng.

### 2.1 Danh sách packet ưu tiên làm trước (để unblock client)

- [ ] Nhóm A - Auth/Session:
  - [ ] Login request/response.
  - [ ] Server list + server select.
- [ ] Nhóm B - Character:
  - [ ] Character list.
  - [ ] Character focused/select.
  - [ ] Enter world/map changed.
- [ ] Nhóm C - Core gameplay:
  - [ ] Object walked/moved.
  - [ ] Hit request/result.
  - [ ] Pickup item request/result.
- [ ] Với mỗi packet mismatch, ghi theo mẫu:
  - [ ] Opcode.
  - [ ] Triệu chứng.
  - [ ] Log liên quan.
  - [ ] Cách tái hiện.
  - [ ] Trạng thái fix.

## 3) Correctness & Authority

- [ ] Đảm bảo server-authoritative cho:
  - [ ] Movement.
  - [ ] Combat.
  - [ ] Drop/loot.
- [ ] Validate state transition:
  - [ ] Chặn packet sai trạng thái.
  - [ ] Chặn spam/rate bất thường.
- [ ] Không để silent failure:
  - [ ] Mọi lỗi quan trọng phải có log và mã lỗi.

## 4) Reliability & Data Safety

- [ ] Kiểm tra cơ chế lưu account định kỳ.
- [ ] Đảm bảo khôi phục sau restart không mất trạng thái quan trọng.
- [ ] Kiểm tra xử lý lỗi DB và reconnect path.
- [ ] Có checklist rollback cấu hình khi deploy lỗi.

## 5) Observability

- [ ] Theo dõi metrics tối thiểu:
  - [ ] Online player count.
  - [ ] Packet error rate.
  - [ ] Disconnect reason.
  - [ ] RTT trung bình.
- [ ] Thiết lập cảnh báo:
  - [ ] Login fail tăng đột biến.
  - [ ] Crash loop.
  - [ ] DB connection issue.

## 6) Milestone Done Criteria

- [ ] Milestone 1: local boot + login/map pass.
- [ ] Milestone 2: vertical slice chạy ổn định.
- [ ] Milestone 3: hỗ trợ client cross-platform alpha.

## 7) Definition of Done cho 2 tuần đầu

- [ ] Tuần 1 (Server Local Boot):
  - [ ] Docker all-in-one chạy ổn định.
  - [ ] Admin panel truy cập được.
  - [ ] Connect + Game server đã start thành công.
  - [ ] Login và vào map pass ít nhất 3 lần liên tiếp.
- [ ] Tuần 2 (Season 6 Baseline):
  - [ ] Packet nhóm A/B ổn định.
  - [ ] Packet nhóm C chạy được cho movement/combat/pickup cơ bản.
  - [ ] Có danh sách mismatch và kế hoạch xử lý tuần kế tiếp.
