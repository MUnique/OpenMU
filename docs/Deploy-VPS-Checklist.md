# Deploy VPS Checklist (OpenMU)

Mục tiêu: chuẩn hóa quy trình triển khai OpenMU lên VPS theo hướng an toàn, có thể rollback, và dễ vận hành lâu dài.

## 1) Pre-Deploy Gate (Local/Staging)

- [ ] Code đã merge vào nhánh phát hành (ví dụ: `main` hoặc `release/*`).
- [ ] Build thành công:
  - [ ] `dotnet build src/Startup/MUnique.OpenMU.Startup.csproj -c Release`
- [ ] Server smoke test pass:
  - [ ] Admin Panel truy cập được.
  - [ ] Connect/Game listeners lên đúng cổng.
  - [ ] Không có exception nghiêm trọng lặp lại trong log.
- [ ] Chốt version phát hành:
  - [ ] Tag git (ví dụ: `v0.1.0`).
  - [ ] Tag image (ví dụ: `ghcr.io/<org>/openmu:v0.1.0`).

## 2) Container Build & Registry

- [ ] Chọn registry:
  - [ ] Docker Hub / GHCR / private registry.
- [ ] Build image từ source:
  - [ ] `docker build -f src/Startup/Dockerfile -t <registry>/openmu:<tag> src`
- [ ] Push image:
  - [ ] `docker push <registry>/openmu:<tag>`
- [ ] Lưu immutable reference:
  - [ ] Ghi lại image digest (`sha256:...`) để deploy chính xác.

## 3) VPS Baseline Setup

- [ ] VPS đã cài:
  - [ ] Docker Engine
  - [ ] Docker Compose plugin
- [ ] Mở firewall/security group:
  - [ ] `80` (Admin Panel qua reverse proxy)
  - [ ] `44405-44406` (Connect Server)
  - [ ] `55901-55906` (Game Server)
  - [ ] `55980` (Chat Server)
- [ ] Đồng bộ timezone/NTP trên VPS.
- [ ] Có volume bền vững cho database/log:
  - [ ] Postgres data volume
  - [ ] log volume (khuyến nghị)

## 4) Production Configuration

- [ ] Chuẩn bị file `.env` trên VPS (không commit):
  - [ ] `DB_HOST`
  - [ ] `DB_ADMIN_USER`
  - [ ] `DB_ADMIN_PW`
  - [ ] `ASPNETCORE_URLS`
  - [ ] `RESOLVE_IP` (public/local/custom theo hạ tầng)
- [ ] Đổi credentials mặc định:
  - [ ] Admin Panel user/password
  - [ ] DB password mặc định
- [ ] Chốt mode IP resolving:
  - [ ] Public IP / DNS thật cho user internet.

## 5) Deploy Execution (VPS)

- [ ] Kéo cấu hình deploy về VPS (compose file + env + scripts).
- [ ] Pull image đúng tag:
  - [ ] `docker compose pull`
- [ ] Chạy dịch vụ:
  - [ ] `docker compose up -d`
- [ ] Kiểm tra trạng thái:
  - [ ] `docker compose ps`
  - [ ] `docker compose logs --tail=200 openmu-startup`
- [ ] Verify sau deploy:
  - [ ] Admin Panel truy cập được.
  - [ ] Connect/Game server ở trạng thái Started.
  - [ ] Client login/select server/map pass (test account nội bộ).

## 6) Database Safety

- [ ] Backup DB trước deploy:
  - [ ] `pg_dump` hoặc snapshot volume.
- [ ] Nếu có migration/schema change:
  - [ ] test migration ở staging trước.
  - [ ] có kế hoạch rollback DB rõ ràng.
- [ ] Backup retention policy:
  - [ ] daily backup.
  - [ ] giữ tối thiểu 7-14 bản gần nhất.

## 7) Rollback Plan (Bắt buộc)

- [ ] Giữ lại image tag stable gần nhất.
- [ ] Nếu lỗi sau deploy:
  - [ ] `docker compose down`
  - [ ] set lại image tag stable
  - [ ] `docker compose up -d`
- [ ] Nếu lỗi do schema:
  - [ ] restore DB backup tương ứng.
- [ ] Checklist xác nhận sau rollback:
  - [ ] listeners lên lại.
  - [ ] login/map ổn định.

## 8) Observability & Operations

- [ ] Thu thập log tập trung (ít nhất file rotation).
- [ ] Theo dõi metrics cơ bản:
  - [ ] online player count
  - [ ] packet error rate
  - [ ] disconnect reason
  - [ ] CPU/RAM/DB connections
- [ ] Thiết lập alert:
  - [ ] service down
  - [ ] login fail tăng đột biến
  - [ ] crash loop

## 9) Security Hardening

- [ ] Không expose dịch vụ không cần public.
- [ ] Chỉ mở đúng cổng cần thiết.
- [ ] Bật HTTPS/reverse proxy nếu public Admin Panel.
- [ ] Định kỳ rotate password/secret.
- [ ] Không lưu secrets trong repository.

## 10) Post-Deploy Checklist

- [ ] Ghi release note:
  - [ ] commit range
  - [ ] tính năng chính
  - [ ] known issues
- [ ] Ghi deployment record:
  - [ ] thời gian deploy
  - [ ] người deploy
  - [ ] image tag/digest
  - [ ] kết quả verify
- [ ] Theo dõi sau deploy 30-60 phút:
  - [ ] không có lỗi nghiêm trọng
  - [ ] các chỉ số ổn định

## 11) Lệnh mẫu tham khảo

```bash
# Build image
docker build -f src/Startup/Dockerfile -t ghcr.io/your-org/openmu:v0.1.0 src

# Push image
docker push ghcr.io/your-org/openmu:v0.1.0

# VPS deploy
docker compose pull
docker compose up -d
docker compose ps
docker compose logs --tail=200 openmu-startup
```
