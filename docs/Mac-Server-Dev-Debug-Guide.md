# Mac Server Dev & Debug Guide (No Game Client)

Mục tiêu: cho phép bạn dev/chỉnh sửa/debug OpenMU trên mac trước, chưa cần test gameplay bằng client Windows.

## 1) Chế độ làm việc đề xuất trên mac

- Dùng Docker để chạy server stack nhanh và ổn định.
- Dùng Admin Panel + logs + port checks để xác minh hành vi server.
- Khi cần debug code C#, chạy từ source với .NET SDK 10.

## 2) Smoke test server trên mac (không cần game client)

Thực hiện trong thư mục `deploy/all-in-one`:

- [ ] Khởi động stack:
  - [ ] `docker compose up -d --no-build`
- [ ] Kiểm tra container:
  - [ ] `docker compose ps`
- [ ] Kiểm tra log startup:
  - [ ] `docker compose logs --tail=150 openmu-startup`
  - [ ] Kỳ vọng có các dòng:
    - [ ] database initialization finished
    - [ ] Game Server initialized
    - [ ] Client Listener started (44405, 44406)
- [ ] Kiểm tra HTTP endpoint:
  - [ ] `curl -I http://localhost:8081/` trả `200 OK`
  - [ ] `curl -I http://localhost/` trả `401 Unauthorized` (qua nginx auth)
- [ ] Kiểm tra cổng listener:
  - [ ] `nc -vz 127.0.0.1 44405`
  - [ ] `nc -vz 127.0.0.1 44406`
  - [ ] `nc -vz 127.0.0.1 55901`

## 3) Cấu hình Admin Panel cần giữ đúng

Trong `Game Configuration -> System`:

- [ ] `IP Resolving = Local` (hoặc `loopback` nếu dùng start arg/env)
- [ ] `Auto Start = enabled`
- [ ] Save cấu hình

Trong `Servers`:

- [ ] Có ít nhất 1 Game Server ở trạng thái `Started`
- [ ] Có 2 Connect Server ở trạng thái `Started`

## 4) Debug từ source trên mac (khi bắt đầu sửa code)

Yêu cầu:

- [ ] Cài .NET SDK 10
- [ ] Cài Node.js 16+
- [ ] PostgreSQL (hoặc dùng DB trong Docker)

Chạy từ source:

- [ ] Mở `src/MUnique.OpenMU.sln`
- [ ] Chạy project `Startup/MUnique.OpenMU.Startup.csproj`
- [ ] Start params khuyến nghị:
  - [ ] `-autostart -resolveIP:loopback`

Điểm đặt breakpoint trước:

- [ ] `src/GameServer/MessageHandler/*`
- [ ] `src/GameLogic/PlayerActions/*`
- [ ] `src/ConnectServer/*`

## 5) Bộ test tối thiểu trước khi qua mobile/client

- [ ] Restart stack 2 lần liên tiếp không lỗi init DB
- [ ] Admin Panel truy cập ổn định sau restart
- [ ] Listeners 44405/44406/55901 luôn mở sau restart
- [ ] Không có exception nghiêm trọng trong `openmu-startup` logs (5-10 phút idle)

## 6) Lệnh hữu ích

- Xem log realtime:
  - `docker compose logs -f openmu-startup`
- Restart toàn stack:
  - `docker compose down && docker compose up -d --no-build`
- Dừng toàn bộ:
  - `docker compose down`
