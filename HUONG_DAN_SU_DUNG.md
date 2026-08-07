# Allflame Bot 0.23.20

Yêu cầu DreamPoeBot 0.3.29.11 trở lên. Bản này đã được compile với
DreamPoeBot 0.3.29.12 và dùng API Deepwater native để đọc Chart Device,
kiểm tra chart, kiểm tra trạng thái Descend và thực hiện Descend.

Bot tự động chạy Chart Area của nội dung Allflame/Deepwater trên
DreamPoeBot 3.29.2.

## 1. Cài đặt

1. Thoát hoàn toàn DreamPoeBot.
2. Mở thư mục cài DreamPoeBot.
3. Chép nguyên thư mục `AllflameBot` vào thư mục `3rdParty`.
4. Kết quả phải có đường dẫn:
   `DreamPoeBot\3rdParty\AllflameBot\3rdparty.json`.
5. Mở lại DreamPoeBot và đợi bộ nạp `3rdParty` biên dịch.
6. Chọn `Allflame Bot` trong danh sách Bots.
7. Chọn `Allflame Mover` trong danh sách PlayerMovers.

Không cần TheThief, Heist Mover hay Deepwater Probe để chạy bot.
Deepwater Probe chỉ nên bật khi cần thu thập dữ liệu chẩn đoán.

## 2. Chuẩn bị trong game

- Nhân vật phải mở khóa The Sovereign, Valerie và Chart Area.
- Đặt Chart Map trong inventory hoặc cấu hình Voyage Stash.
- Chừa đủ ô trống trong inventory.
- Đặt Life Flask và Utility Flask đúng các slot đã cấu hình.
- Nên dùng cửa sổ game ở chế độ mà DreamPoeBot hỗ trợ ổn định.
- Không điều khiển chuột trong lúc bot đang chạy.

## 3. Cấu hình khởi đầu đề xuất

Trong tab `General`:

- Bật `Automatically run Chart Areas`.
- Muốn chỉ dùng Chart đang có trong inventory thì bật
  `Run Charts from inventory only`.
- Muốn lấy 20 Chart từ Voyage Stash khi inventory hết Chart thì bật
  `Check Voyage Stash tab first`.
- Giữ `Interaction distance` ở mức 18.
- Bật Auto-Lantern, phím V dự phòng và quay về khi Lantern cạn.
- Chọn các loại chest cần mở và đặt priority từ 0 đến 100.
  Số lớn hơn được ưu tiên trước.

Trong tab `Stash`:

- Bật `Enable stashing`.
- Nhập tên hoặc số tab vào `Loot tab names/numbers`, ngăn cách bằng dấu phẩy.
- Đặt ngưỡng `Stash when free slots reach`.
- Nếu dùng Voyage Stash, nhập đúng tên hoặc số tab chứa Chart.
- Nút `DEBUG: Stash non-Chart inventory` chỉ hoạt động khi bot đang chạy
  trên The Sovereign. Bot giữ lại toàn bộ Chart Map.

Trong tab `Loot`:

- Bật `Loot after opening chests`.
- Bật `Only loot items visible by the in-game filter` nếu muốn tôn trọng
  loot filter trong game.
- Chọn các nhóm vật phẩm cần nhặt trong `PICKIT ITEM LOGIC`.
- Bật `All Scarabs` nếu muốn nhặt Scarab.
- `Walk over Gold` khiến bot đi qua Gold thay vì click từng chồng.
- Bot chỉ xét vật phẩm mới xuất hiện từ chest đang xử lý; vật phẩm từ quái
  nằm sẵn trước đó sẽ bị bỏ qua.

Trong tab `Extra`:

- Bật Life Flask và đặt ngưỡng máu phù hợp.
- Utility Flask dùng số slot trong game từ 1 đến 5.
- Nên bật `Disable utility flasks on boat`.

Trong tab `Prices`:

- Giữ `Enable item pricing` để bot tính giá trị loot và lợi nhuận.
- `League name` mặc định là `Allflame`.
- Bật `Update prices automatically` để cập nhật cache theo chu kỳ.
- Có thể bấm `Update prices now` để cập nhật ngay.
- Giá cố định dùng định dạng `Tên item=giá divine; Item khác=giá divine`.
- `Dead Man's Sulphur` được lấy giá tự động từ nguồn Currency. Có thể ghi đè
  giá mỗi đơn vị, ví dụ `Dead Man's Sulphur=0.00002074`.
- Cache được lưu tại `Settings\AllflameBot\prices.json`; nếu cập nhật lỗi,
  bot tiếp tục dùng cache cũ.

Trong tab `Notifications`:

- Dán Discord webhook HTTPS vào `Discord webhook URL`; ô nhập mặc định được
  che và chỉ cho phép endpoint Discord hợp lệ.
- Bật `Enable Discord notifications`.
- Bấm `Send test notification` để kiểm tra webhook.
- `Refresh dashboard periodically` cập nhật một message dashboard duy nhất;
  có thể đặt chu kỳ bằng `Refresh interval (minutes)` hoặc bấm `Send dashboard
  now`.
- Dashboard hiển thị trạng thái Chart Runner/Voyage Runner, area, Lantern,
  Sulphur, Chart inventory, Chart Storage, Voyage Stash, loot đã xác nhận và
  profit.
- Muốn tag toàn bộ server khi có đồ giá trị, bật `Notify valuable confirmed
  loot`
  và `Tag @everyone for valuable items`, sau đó đặt `Minimum value (Divine
  Orbs`. Có thể dùng giá trị lẻ như `0.5`.
- Alert `@everyone` chỉ được gửi sau khi pickup đã xác nhận; dashboard session
  vẫn được edit bình thường.
- Nhóm `Chart and Voyage Supply` điều khiển cảnh báo Chart thấp/hết và Voyage
  Stash thấp. Nhóm `Loot and Rewards` điều khiển loot giá trị và Bottle reward.
- Nhóm `Run and Reliability Alerts` điều khiển price warning, death, recovery
  failure và terminal stop.
- Notification chạy qua queue nền, không giữ movement/task cycle và không tự
  quyết định stop bot.

Bot chỉ cộng item sau khi xác nhận item đã biến mất khỏi mặt đất. Vì vậy các
lần click retry không làm tăng trùng số lượng hoặc lợi nhuận.

## 4. Cách chạy

1. Đứng ở hideout hoặc The Sovereign.
2. Đảm bảo inventory có Chart hoặc Voyage Stash đã được cấu hình.
3. Chọn `Allflame Bot` và `Allflame Mover`.
4. Nhấn Start.
5. Bot sẽ dùng `/boat`, vào vị trí chuẩn trên tàu, chuẩn bị Chart,
   mở Chart Device, nhấn Descend và đi vào Bathysphere.
6. Trong Chart Area, bot ưu tiên Lantern cạn, chest đã chọn và loot từ chest.
7. Khi Lantern về 0, bot quay ngược đường đã ghi nhận để trở lại Bathysphere.

Nếu inventory đang có 20 Chart, bot sẽ chạy hết số Chart còn lại trước khi
mở Voyage Stash để lấy đợt mới.

## 5. Kiểm tra lỗi

Log gọn của bot nằm trong:

`DreamPoeBot\Logs\AllflameBot`

Khi cần báo lỗi, gửi:

- Video từ trước khi lỗi khoảng 20–30 giây.
- Log AllflameBot mới nhất.
- Log DreamPoeBot cùng thời điểm.
- Ảnh tab General, Stash và Loot đang sử dụng.

Nếu bot không xuất hiện trong danh sách:

1. Kiểm tra đúng đường dẫn `3rdParty\AllflameBot\3rdparty.json`.
2. Xóa bản AllflameBot cũ nếu đang có hai bản trùng nhau.
3. Khởi động lại hoàn toàn DreamPoeBot.
4. Kiểm tra log tải `3rdParty` để tìm lỗi biên dịch.

## 6. Prices và notifications

- Giá được lưu theo đơn vị Divine Orb.
- `Total Profit` = giá trị loot trong rương + giá trị `Dead Man's Sulphur` đã
  xác nhận nhặt trong phiên hiện tại.
- `Profit per hour` dùng thời gian thực của phiên bot.
- Item chưa có giá vẫn xuất hiện trong `Items Found` và được đếm trong
  `Unpriced item types`.
- Sulphur chỉ được cộng khi vật phẩm trên mặt đất đã biến mất sau thao tác
  nhặt. Dashboard hiển thị cả số lượng, giá trị Divine và phần đóng góp vào
  total profit; không tạo card `Sulphur Collected` và không gắn Sulphur với
  `Chart #`.
- Không chia sẻ webhook Discord trong ảnh hoặc log gửi cho người khác.

## 7. Giới hạn hiện tại

- Voyage ghép chín Chart và Capsule automation chưa phải luồng hoàn thiện.
- Khả năng nhận dạng phụ thuộc dữ liệu UI và object mà phiên bản DPB hiện tại
  cung cấp.
- Cần kiểm thử lại sau mỗi lần DreamPoeBot hoặc Path of Exile cập nhật.
