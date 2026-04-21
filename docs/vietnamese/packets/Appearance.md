# Appearance

Sự xuất hiện của các ký tự được tuần tự hóa như sau.
Webzen đã làm rất tốt việc tiết kiệm một chút chỗ này chỗ kia nên hơi phức tạp
đôi khi (ví dụ như đôi cánh và thú cưng).
Có vẻ như cấu trúc này đã được phát triển trong lịch sử.

## Cấu trúc

Vui lòng đọc bảng này như một dòng bit. Ví dụ. nếu có 8 bit của 1 byte
được chỉ định trong một hàng, các bit cao nhất sẽ xuất hiện trước.

| Byte index | Length | Data type | Description |
|----------|---------|-------------|------------|
| 0 | 4 | bit | Lớp nhân vật |
| 0 | 4 | bit | Tư thế nhân vật, xem bên dưới |
| 1 | 1 | byte | Chỉ số mục bên trái. 0xFF nếu trống. |
| 2 | 1 | byte | Chỉ số mục bên tay phải. 0xFF nếu trống. |
| 3 | 4 | bit | Chỉ số mục Helm (4 bit thấp hơn). Xem chỉ mục byte 9 và 13 để biết phần còn lại. |
| 3 | 4 | bit | Chỉ số vật phẩm áo giáp (4 bit thấp hơn). Xem chỉ mục byte 9 và 14 để biết phần còn lại. |
| 4 | 4 | bit | Chỉ số mục quần (4 bit thấp hơn). Xem chỉ mục byte 9 và 14 để biết phần còn lại. |
| 4 | 4 | bit | Chỉ số mục găng tay (4 bit thấp hơn). Xem chỉ mục byte 9 và 15 để biết phần còn lại. |
| 5 | 4 | bit | Chỉ số mục khởi động (4 bit thấp hơn). Xem chỉ mục byte 9 và 15 để biết phần còn lại. |
| 5 | 4 | bit | Cờ thú cưng và cánh, xem bên dưới |
| 6 | 3 | bit | Cấp độ vật phẩm bên tay trái |
| 6 | 3 | bit | Cấp độ vật phẩm bên tay phải |
| 6~7 | 3 | bit | Cấp độ vật phẩm mũ bảo hiểm |
| 7 | 3 | bit | Cấp độ vật phẩm áo giáp |
| 7 | 3 | bit | Cấp độ vật phẩm quần |
| 7~8 | 3 | bit | Cấp độ vật phẩm găng tay |
| 8 | 3 | bit | Cấp độ vật phẩm khởi động |
| 8 | 3 | bit | chưa sử dụng |
| 9 | 1 | bit | Chỉ số vật phẩm mũ bảo hiểm (bit thứ 5) |
| 9 | 1 | bit | Chỉ số vật phẩm áo giáp (bit thứ 5) |
| 9 | 1 | bit | Chỉ số mục quần (bit thứ 5) |
| 9 | 1 | bit | Chỉ số mục găng tay (bit thứ 5) |
| 9 | 1 | bit | Chỉ mục mục khởi động (bit thứ 5) |
| 9 | 3 | bit | Chỉ số mục cánh (xem bảng bên dưới) |
| 10 | 1 | bit | Cờ tùy chọn tuyệt vời của Helm |
| 10 | 1 | bit | Cờ tùy chọn tuyệt vời của áo giáp |
| 10 | 1 | bit | Cờ lựa chọn tuyệt vời của quần |
| 10 | 1 | bit | Cờ tùy chọn tuyệt vời của găng tay |
| 10 | 1 | bit | Khởi động cờ tùy chọn tuyệt vời |
| 10 | 1 | bit | Cờ tùy chọn tuyệt vời của mặt hàng bên trái |
| 10 | 1 | bit | Cờ tùy chọn tuyệt vời của vật phẩm bên tay phải |
| 10 | 1 | bit | Cờ khủng long |
| 11 | 1 | bit | Cờ tùy chọn cổ Helm |
| 11 | 1 | bit | Cờ tùy chọn áo giáp cổ xưa |
| 11 | 1 | bit | Cờ tùy chọn quần cổ |
| 11 | 1 | bit | Găng tay tùy chọn cờ cổ |
| 11 | 1 | bit | Cờ tùy chọn khởi động cổ xưa |
| 11 | 1 | bit | Cờ tùy chọn cổ bên trái |
| 11 | 1 | bit | Cờ tùy chọn cổ bên tay phải |
| 11 | 1 | bit | Toàn bộ cờ cổ |
| 12 | 3 | bit | Nhóm vật phẩm bên trái. 111 = trống |
| 12 | 1 | bit | cờ không sử dụng hoặc trống? |
| 12 | 1 | bit | chưa sử dụng |
| 12 | 1 | bit | Cờ Fenrir |
| 12 | 1 | bit | chưa sử dụng |
| 12 | 1 | bit | Cờ ngựa đen |
| 13 | 3 | bit | Nhóm vật phẩm bên tay phải. 111 = trống |
| 13 | 1 | bit | cờ không sử dụng hoặc trống? |
| 13 | 4 | bit | Chỉ số mục mũ bảo hiểm (bit thứ 6-9). 0xF = trống |
| 14 | 4 | bit | Chỉ số vật phẩm áo giáp (bit thứ 6-9). 0xF = trống |
| 14 | 4 | bit | Chỉ số mục quần (bit thứ 6-9). 0xF = trống |
| 15 | 4 | bit | Chỉ số mục găng tay (bit thứ 6-9). 0xF = trống |
| 15 | 4 | bit | Chỉ mục mục khởi động (bit thứ 6-9). 0xF = trống |
| 16 | 6 | bit | Chỉ số vật phẩm thú cưng, xem bên dưới |
| 16 | 1 | bit | Cờ Fenrir màu xanh |
| 16 | 1 | bit | Cờ Fenrir đen |
| 17 | 4 | bit | Chỉ số vật phẩm Cánh nhỏ, xem bên dưới |
| 17 | 3 | bit | chưa sử dụng |
| 17 | 1 | bit | Cờ Fenrir vàng |

### Character pose

| Value | Meaning |
|-------|---------|
|   0   | Standing (default) |
|   1   | unused  |
|   2   | Sitting (e.g. on a trunk) |
|   3   | Leaning (e.g. against a wall) |
|   4   | Hanging (at these strange things in Noria) |

Lưu ý: Ở Phần 10 trở lên, các giá trị sẽ khác nhau.
Ngồi là 1, Nghiêng 2, Treo 3. Vì vậy, họ loại bỏ giá trị không sử dụng.

### Item level calculation

Cấp độ vật phẩm được tính theo công thức sau: ([Cấp vật phẩm] - 1) / 2.

Điều này hoạt động cho đến cấp độ mục 15, vì nó phù hợp với 4 bit.

### Item indexes

Các byte được biểu diễn ở định dạng nhị phân. X có nghĩa là bit được sử dụng bởi thứ gì đó
khác (xem bảng trên).

#### Pet items

| Item | 5th byte | 10th byte | byte thứ 12 |
|-----------------------|----------|-----------|-----------|
| Guardian Angel        | xxxxxx00 |
| Imp                   | xxxxxx01 |
| Unicorn               | xxxxxx10 |
| Dinorant | xxxxxx11 | xxxxxxx1 |  |
| Fenrir | xxxxxx11 |  | xxxxxx1x |
| None | xxxxxx11 | xxxxxxx0 |

Và một số thú cưng khác:

| Item                  | 16th byte |
|-----------------------|-----------|
| Pet Panda             | 111000xx  |
| Pet Unicorn           | 101000xx  |
| Skeleton              | 011000xx  |
| Rudolph               | 100000xx  |
| Spirit of Guardian    | 010000xx  |
| Demon                 | 001000xx  |

#### Wings

| Item | Character Class | 5th byte | byte thứ 9 |
|-----------------------|-----------------|----------|----------|
| Wings of Elf | Fairy Elf | xxxx01xx | xxxxx001 |
| Wings of Heaven | Dark Wizard | xxxx01xx | xxxxx010 |
| Wings of Satan | Dark Knight | xxxx01xx | xxxxx011 |
| Wings of Mistery | Summoner | xxxx01xx | xxxxx100 |
| Wings of Spirit | Muse Elf | xxxx10xx | xxxxx001 |
| Wings of Soul | Soul Master | xxxx10xx | xxxxx010 |
| Wings of Dragon | Blade Knight | xxxx10xx | xxxxx011 |
| Wings of Darkness | Magic Gladiator | xxxx10xx | xxxxx100 |
| Cape of Lord | Dark Lord | xxxx10xx | xxxxx101 |
| Wings of Despair | Bloody Summoner | xxxx10xx | xxxxx110 |
| Cape of Fighter | Rage Fighter | xxxx10xx | xxxxx111 |
| Wing of Storm | Blade Master | xxxx11xx | xxxxx001 |
| Wing of Eternal | Grand Master | xxxx11xx | xxxxx010 |
| Wing of Illusion | High Elf | xxxx11xx | xxxxx011 |
| Wing of Ruin | Duel Master | xxxx11xx | xxxxx100 |
| Cape of Emperor | Lord Emperor | xxxx11xx | xxxxx101 |
| Wing of Dimension | Dimension Master | xxxx11xx | xxxxx110 |
| Cape of Overrule | Fist Master | xxxx11xx | xxxxx111 |
| None |  | xxxx00xx | xxxxx000 |

#### Small Wings

| Wing Item | 5th byte | byte thứ 17 |
|-----------------------|----------|-------|
| Small Cape of Lord | xxxx11xx | 0x20 |
| Small Wings of Mistery | xxxx11xx | 0x40 |
| Small Wings of Elf | xxxx11xx | 0x60 |
| Small Wings of Heaven | xxxx11xx | 0x80 |
| Small Wings of Satan | xxxx11xx | 0xA0 |
| Small Cloak of Warrior | xxxx11xx | 0xC0 |