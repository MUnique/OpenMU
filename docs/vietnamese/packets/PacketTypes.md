# Loại gói

Tùy thuộc vào byte đầu tiên của gói, gói có các thuộc tính khác nhau.

| First byte | Độ dài của gói | Encrypted Server -> Client | Encrypted Client -> Server |
|------------|----------------------|-----------|---------|
| 0xC1 | Được chỉ định bởi byte thứ hai | No | Có (XOR32) |
| 0xC2 | Được chỉ định bởi byte thứ hai và thứ ba | No | Có (XOR32) |
| 0xC3 | Được chỉ định bởi byte thứ hai | Có (Mô-đun đơn giản) | Có (Mô đun đơn giản + XOR32) |
| 0xC4 | Được chỉ định bởi byte thứ hai và thứ ba | Có (Mô-đun đơn giản) | Có (Mô đun đơn giản + XOR32) |