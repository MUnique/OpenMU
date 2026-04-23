# Week 1 Execution Checklist (Client MU Mobile)

## Day 1 - Environment and baseline

- [ ] Cài Unity 6 LTS + Android Build Support.
- [ ] Clone repo client (fork hoặc repo mới).
- [ ] Tạo project theo `docs/Unity-Project-Structure-Template.md`.
- [ ] Xác nhận OpenMU local đang chạy (connect ports 44405/44406).

## Day 2 - Module boundaries and bootstrap

- [ ] Tạo asmdef:
  - [ ] `OpenMU.Client.Core`
  - [ ] `OpenMU.Client.Protocol`
  - [ ] `OpenMU.Client.Runtime.Unity`
  - [ ] `OpenMU.Client.Tools`
- [ ] Tạo scene `Bootstrap.unity` và `Login.unity`.
- [ ] Wire DI/bootstrap để Runtime chỉ gọi Core qua interface.

## Day 3 - Protocol MVP: Login

- [ ] Implement `LoginRequestPacket` encode.
- [ ] Implement `LoginResponsePacket` decode.
- [ ] Log packet hex trước/sau encode-decode để kiểm tra.
- [ ] Connect thành công tới OpenMU và nhận response.

## Day 4 - Input + mobile readiness

- [ ] Tích hợp Unity Input System.
- [ ] Tạo input adapter desktop/mobile dùng chung command model.
- [ ] Tích hợp joystick package cơ bản cho mobile.

## Day 5 - Replay scaffold integration

- [ ] Đồng bộ với `tests/protocol-replay/*` trong repo server.
- [ ] Tạo utility đọc `stream.hex.txt`.
- [ ] Viết test replay đầu tiên cho flow login.
- [ ] Fail test nếu packet sequence mismatch.

## Day 6 - Android smoke build

- [ ] Build Android debug (APK) và cài lên thiết bị thật.
- [ ] Test login trên cùng mạng LAN với OpenMU server.
- [ ] Ghi nhận FPS, frame time, ping ban đầu.

## Day 7 - Review gate

- [ ] Demo live flow login end-to-end.
- [ ] Chốt blockers kỹ thuật tuần 2.
- [ ] Cập nhật ADR/Checklist theo thực tế.

## Week 1 Done Definition

- [ ] Có project Unity chạy được.
- [ ] Có module boundary đúng ADR.
- [ ] Login protocol chạy được với OpenMU.
- [ ] Có Android build smoke test.
- [ ] Có ít nhất 1 replay test chạy pass.
