# Hệ thống Master

## Master là gì?

Khi nhân vật đạt một mốc level nhất định (thường là level 400) và hoàn thành
quest tương ứng, class sẽ chuyển thành *master character class*.

Khi đó, người chơi mở khóa *Master Skill Tree* để phân phối điểm vào các
master skill. Mỗi master level tăng lên sẽ nhận thêm điểm (tương tự cơ chế
tăng level bằng exp trước đó).

## Các loại master skill

Có hai loại chính trong một cây master skill:

### Passive skills

Khi học, các skill này cộng chỉ số thụ động. Ví dụ:

- tăng máu tối đa
- tăng sát thương tấn công

### Active skills

Khi học, skill xuất hiện trong danh sách kỹ năng.
Phần lớn skill loại này thay thế các skill cũ. Skill cũ vẫn tồn tại ở nền để
giữ logic nội bộ; master skill chỉ bổ sung thêm damage hoặc buff vào skill đã
bị thay thế.

## Cấu trúc cây master skill

Cây master gồm 3 nhánh gốc (roots).
Skill được chia theo hàng để xác định *rank*.

- Mặc định có 5 rank.
- Client hỗ trợ tới 9 rank.
- Mỗi skill thường có tối đa 20 level, một số skill chỉ có 10.

Skill có thể phụ thuộc vào skill cùng root ở cùng rank hoặc rank trước đó.

Mỗi class có danh sách skill khác nhau, có skill riêng theo class và có skill
dùng chung nhiều class. Tuy nhiên, root và rank của một skill luôn nhất quán
giữa các class; chỉ phần hiển thị phía client có thể khác.

## Điều kiện để học skill

Server (và client) kiểm tra các điều kiện sau:

### Character class

Skill phải được định nghĩa cho class hiện tại của nhân vật.

### Rank

Skill phải ở rank đầu, hoặc skill ở rank trước cùng root phải đạt ít nhất
level 10.

### Required skill

Nếu skill có khai báo required skills, các skill đó phải đạt ít nhất level 10.

## Triển khai phía client

Một số lưu ý ngắn:

- Có packet gửi thông tin master level (F3 50).
  - Packet này chứa cả health/mana. Nếu không gửi, nhân vật master có thể hiển
    thị thiếu health/mana.
- Có packet gửi danh sách master skills đã học (F3 53).
  - Mỗi skill gồm:
    - Skill Number
    - Skill Index (vị trí hiển thị trong UI client)
    - Current Level
    - Giá trị hiệu ứng hiện tại
    - Giá trị hiệu ứng ở level kế tiếp
  - Dù chưa học skill nào, vẫn cần gửi packet này để tránh client giữ dữ liệu
    cũ từ nhân vật master trước đó.

## Triển khai phía server

### Cộng điểm master

- Code: [AddMasterPointAction](https://github.com/MUnique/OpenMU/tree/master/src/GameLogic/PlayerActions/Character/AddMasterPointAction.cs)
- Request packet: [C1F352 - Add Master Skill Point](../Packets/C1-F3-52-AddMasterSkillPoint_by-client.md)
- Response packet: [C1F352 - Master skill level update](../Packets/C1-F3-52-MasterSkillLevelUpdate_by-server.md)

### Gửi master stats (F3 50)

- Code: [UpdateMasterStatsPlugIn](https://github.com/MUnique/OpenMU/tree/master/src/GameServer/RemoteView/Character/UpdateMasterStatsPlugIn.cs)
- Packet: [C2F350 - Master Stats Update](../Packets/C1-F3-50-MasterStatsUpdate_by-server.md)

### Gửi master skills (F3 53)

- Code: [UpdateMasterSkillsPlugIn](https://github.com/MUnique/OpenMU/tree/master/src/GameServer/RemoteView/Character/UpdateMasterSkillsPlugIn.cs)
- Packet: [C2F353 - Master Skill List](../Packets/C2-F3-53-MasterSkillList_by-server.md)
