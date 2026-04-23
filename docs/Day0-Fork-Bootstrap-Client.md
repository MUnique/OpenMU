# Day 0 Fork & Bootstrap (Client MU)

## Repo fork chính thức hôm nay

- **Primary fork:** `bernatvadell/muonline`
  - URL: https://github.com/bernatvadell/muonline
  - Lý do:
    - Active gần đây, có nhiều contributor.
    - Có sẵn hướng đa nền tảng (PC/macOS/Android/iOS).
    - Đã định hướng chạy với OpenMU nên rút ngắn thời gian tích hợp.

## Day 0 checklist (bấm chạy ngay)

### A. Fork + clone

- [ ] Fork `bernatvadell/muonline` về GitHub org/team.
- [ ] Đặt tên repo: `openmu-client-mobile` (hoặc chuẩn naming của team).
- [ ] Clone local:

  ```bash
  git clone <your-fork-url> openmu-client-mobile
  cd openmu-client-mobile
  ```

- [ ] Thêm upstream:

  ```bash
  git remote add upstream https://github.com/bernatvadell/muonline.git
  git remote -v
  ```

### B. Branch strategy ban đầu

- [ ] Tạo branch tích hợp:

  ```bash
  git checkout -b integration/openmu-bootstrap
  ```

- [ ] Bật branch protection cho `main` (PR required).

### C. Toolchain tối thiểu

- [ ] Cài .NET 10 SDK.
- [ ] Cài workload mobile cần thiết:

  ```bash
  dotnet workload restore
  ```

- [ ] Với macOS/iOS:
  - [ ] Cài Xcode + command line tools.
  - [ ] `xcode-select --install` (nếu chưa có).

### D. First build smoke

- [ ] Restore + build desktop trước:

  ```bash
  dotnet restore
  dotnet build /p:EnableMobileTargets=false
  ```

- [ ] Cấu hình host OpenMU trong settings về máy local.
- [ ] Chạy smoke desktop client và xác nhận mở app thành công.

### E. Kết nối OpenMU local

- [ ] Đảm bảo OpenMU server đang chạy.
- [ ] Set connect server:
  - Host: IP máy chạy OpenMU
  - Port: `44405` hoặc `44406` (theo profile test).
- [ ] Xác nhận client gửi request đầu tiên (login/connect handshake).

### F. Gắn với docs trong repo OpenMU

- [ ] Link ADR/Checklist:
  - `docs/adr/ADR-Client-Engine-Selection.md`
  - `docs/adr/ADR-Client-Module-Boundaries.md`
  - `docs/adr/ADR-Protocol-Replay-Testing-Strategy.md`
  - `docs/Week1-Execution-Checklist-Client.md`

### G. Done criteria Day 0

- [ ] Fork repo xong + remote upstream đúng.
- [ ] Build desktop pass.
- [ ] Client mở được.
- [ ] Có kết nối packet đầu tiên tới OpenMU local.
