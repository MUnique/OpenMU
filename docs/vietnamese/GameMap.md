# GameMap

Tài liệu này mô tả nhanh cách hoạt động của [game map](../src/GameLogic/GameMap.cs),
đặc biệt là cơ chế quan sát giữa player/NPC.

## AreaOfInterestManager

Mỗi map có một `AreaOfInterestManager` để quản lý subscribe sự kiện giữa các
object khi chúng được thêm, di chuyển hoặc rời map.

Triển khai mặc định là
[BucketAreaOfInterestManager](../src/GameLogic/BucketAreaOfInterestManager.cs),
sử dụng `BucketMap` (mô tả bên dưới).

Trong tương lai có thể có triển khai đơn giản hơn cho các map thường rỗng, hoặc
map cần mọi object đều biết nhau (ví dụ map đấu tay đôi).

## BucketMap

[BucketMap](../src/GameLogic/BucketMap{T}.cs) là cấu trúc dữ liệu 2 chiều.
Map 256x256 được chia thành các [bucket](../src/GameLogic/Bucket{T}.cs), ví dụ
mỗi bucket bao phủ vùng 8x8 tọa độ.

Player (hoặc observer khác) theo dõi các bucket trong phạm vi quan tâm để nhận
sự kiện object vào/ra bucket (player khác, NPC, item rơi).

Khi player di chuyển vào vùng mới (theo "info range"), hệ thống subscribe sự
kiện enter/leave của bucket đó và thêm các object hiện có vào view của player.

## Vì sao dùng bucket thay vì so sánh khoảng cách toàn cục?

Cách đơn giản là giữ danh sách object và mỗi lần di chuyển đều so khoảng cách
với tất cả object khác để quyết định add/remove ở client.

Vấn đề: đây là bài toán gần `O(n^2)` theo số object trên cùng map.
Khi số lượng player đông (ví dụ castle siege), chi phí tăng rất nhanh và gây lag.

Với bucket:

- Tìm kiếm nặng chỉ xảy ra khi player đi vào bucket mới.
- So theo bucket trong phạm vi, không phải so từng object toàn map.
- Đổi lại tốn thêm memory và kém tối ưu trên map trống.

Trong thực tế MU map thường dày đặc monster/player/item, nên đánh đổi này đáng
giá.

## Các ý tưởng khác

Ngoài chia bucket (nơi nhiều ô có thể rỗng), có thể thử các cấu trúc chỉ mục 2D
khác như:

- [R-tree](https://en.wikipedia.org/wiki/R-tree)
- [B-tree](https://en.wikipedia.org/wiki/B-tree) kết hợp
  [Z-ordering](https://en.wikipedia.org/wiki/Z-order_curve)
