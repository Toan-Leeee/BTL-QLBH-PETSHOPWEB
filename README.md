# SieuPet MVC - ASP.NET Core MVC + SQL Server

Dự án này là bộ mã nguồn ASP.NET Core MVC cho website bán thú cưng / phụ kiện và khu vực quản trị, bám theo bộ giao diện bạn gửi. Bản cập nhật này đã hoàn thiện thêm **chức năng quản trị, giỏ hàng, khuyến mãi, đơn hàng** và đổi thao tác xóa sản phẩm sang **ẩn/hiện mềm** để không lỗi khóa ngoại.

## Công nghệ
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core + SQL Server
- Razor Views
- HTML / CSS / JavaScript thuần
- Session-based cart + đăng nhập mẫu

## Chức năng đã hoàn thiện
### Khu vực người dùng
- Trang chủ và danh mục sản phẩm
- Xem chi tiết sản phẩm
- Thêm vào giỏ / mua ngay
- Cập nhật số lượng, xóa khỏi giỏ, áp dụng mã khuyến mãi từ database
- Checkout tạo đơn hàng và tự trừ tồn kho
- Đăng nhập / đăng ký khách hàng

### Khu vực admin
- **Sản phẩm:** tìm kiếm, sửa, thêm mới, ẩn/hiện sản phẩm, theo dõi tồn kho
- **Đơn hàng:** lọc theo trạng thái, xem chi tiết đơn, cập nhật trạng thái / phương thức thanh toán / người nhận
- **Tài khoản:** tìm kiếm, thêm mới, chỉnh sửa tài khoản nhân viên
- **Khuyến mãi:** tạo, sửa, xóa, gán sản phẩm áp dụng, tính trạng thái theo ngày
- **Báo cáo:** doanh thu hoàn tất, số đơn, số sản phẩm hiển thị, số nhân viên

## Cấu trúc chính
- `Models/`: entity theo CSDL
- `ViewModels/`: dữ liệu trình bày cho từng màn hình
- `Controllers/`: xử lý nghiệp vụ và điều hướng
- `Views/`: Razor view giao diện người dùng và admin
- `Data/`: `ApplicationDbContext` + `SeedData`
- `Services/`: giỏ hàng session
- `SQL/SieuPet.sql`: script tạo database + dữ liệu mẫu

## Chạy dự án
1. Cài **.NET SDK 8** và **SQL Server**.
2. Mở file `SQL/SieuPet.sql` và chạy lại toàn bộ script để tạo database mẫu mới.
3. Sửa connection string trong `appsettings.json` nếu cần.
4. Chạy lệnh:
   ```bash
   dotnet restore
   dotnet run
   ```
5. Truy cập `https://localhost:<port>` hoặc `http://localhost:<port>`.

## Tài khoản mẫu
- Admin: `admin@sieupet.vn` / `123456`
- Nhân viên vận hành: `ops@sieupet.vn` / `123456`
- Khách hàng: `khachhang@sieupet.vn` / `123456`

## Ghi chú triển khai
- Dự án dùng session để demo đăng nhập và giỏ hàng, phù hợp bài tập / đồ án.
- Khi đưa production nên chuyển sang ASP.NET Identity + password hash + authorization policy.
- Thao tác **Ẩn sản phẩm** chỉ cập nhật `bTrangThai = 0`, không xóa cứng bản ghi khỏi `tblSanPham`, nên tránh lỗi ràng buộc khóa ngoại với `tblChiTietKhuyenMai`, `tblChiTietDonHang`, `tblChiTietPhieuNhap`.
- Nếu bạn đã chạy database cũ trước đó, nên **drop DB cũ và chạy lại `SQL/SieuPet.sql`** để dữ liệu mẫu đồng bộ với code mới.
