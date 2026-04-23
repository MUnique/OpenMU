# ADR-Protocol-Replay-Testing-Strategy

## Status

Accepted

## Date

2026-04-21

## Context

Dự án OpenMU cần hỗ trợ nhiều client/runtime (PC, mobile, web phase sau).
Rủi ro lớn nhất khi scale client là regression ở packet encode/decode và state sync:

- Sửa một packet handler có thể phá login/map change/combat ở client khác.
- Refactor serializer dễ gây lệch binary contract.
- Bug protocol thường khó thấy ngay nếu chỉ test tay.

Nhóm cần một quality gate tự động, lặp lại được, chạy trong CI để phát hiện
regression sớm.

## Decision

1. Áp dụng **Protocol Replay Testing** làm gate bắt buộc cho `Client.Protocol`.
2. Xây bộ `golden captures` cho các flow cốt lõi Season 6:
   - Login
   - Character list/select/create
   - Enter map
   - Move
   - Attack / skill basic
   - Loot item
3. Mỗi PR có thay đổi protocol/serialization phải chạy:
   - Decode replay test
   - Encode replay test
   - Snapshot diff (binary + semantic fields)
4. Nếu mismatch ngoài whitelist, CI fail.

## Scope

### In scope

- Packet parsing và serialization giữa server OpenMU và client protocol layer.
- Compatibility check theo version/profile được hỗ trợ.
- Tooling cho capture, replay, diff.

### Out of scope

- Gameplay balancing.
- Render/input/audio runtime behavior.
- Network latency simulation nâng cao (xử lý ở test tầng khác).

## Test Design

### 1) Golden captures

- Lưu pcap/raw message stream đã chuẩn hóa vào:
  - `tests/protocol-replay/golden/<flow-name>/`
- Mỗi flow có metadata:
  - client profile
  - expected packet sequence
  - checksum/signature

### 2) Decode replay

- Input: golden binary stream.
- Assert:
  - parse thành message model đúng thứ tự
  - field-level invariants đúng
  - không phát sinh unknown critical packets

### 3) Encode replay

- Input: canonical message model fixtures.
- Assert:
  - binary output khớp golden hoặc khớp rule-based comparator
  - length/header/opcode hợp lệ

### 4) Diff rules

- So sánh theo 2 lớp:
  - Binary diff (byte-to-byte)
  - Semantic diff (field-level)
- Cho phép whitelist trường volatile (timestamp, session id random, nonce).

## CI Policy

- Trigger khi thay đổi:
  - `Client.Protocol`
  - packet definitions
  - serializer/deserializer
  - network message mapping
- Gate:
  - replay suite phải pass 100%
  - nếu update intentional, phải kèm commit cập nhật golden + changelog note

## Operational Rules

- Không overwrite golden tự động trong pipeline chính.
- Golden update chỉ qua lệnh explicit và review bắt buộc.
- Mọi packet mới phải có:
  - fixture tối thiểu
  - replay case tối thiểu cho happy path

## Consequences

### Positive

- Giảm regression protocol khi refactor.
- Tăng tự tin khi mở rộng đa nền tảng.
- Dễ truy vết mismatch bằng diff rõ ràng.

### Negative

- Tốn effort ban đầu để dựng capture/tooling.
- Cần governance tốt để tránh golden bị “chấp nhận sai”.

## Initial Rollout Plan

1. Tuần 1:
   - Dựng replay runner tối thiểu.
   - Tạo golden cho login + character select + enter map.
2. Tuần 2:
   - Bổ sung move + attack + loot.
   - Kết nối CI gate cho nhánh chính.
3. Tuần 3:
   - Bổ sung semantic diff và whitelist rules.
   - Bắt buộc ADR compliance cho mọi thay đổi protocol.

## References

- OpenMU repository:
  https://github.com/MUnique/OpenMU
- OpenMU packet docs:
  https://github.com/MUnique/OpenMU/tree/master/docs/Packets
- MuMain (client protocol baseline):
  https://github.com/sven-n/MuMain
