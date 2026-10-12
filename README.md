# QuanLyNhaHang_UNETI02_TI17A2ND

Hệ thống quản lý nhà hàng và đặt bàn - ASP.NET Core 10 MVC + Entity Framework Core 10 + SQL Server.

## 1. Yêu cầu môi trường
- .NET 10 SDK
- SQL Server / SQL Server LocalDB
- Visual Studio 2022 (17.10+) hoặc Visual Studio Code

## 2. Cấu trúc thư mục chính
```
QuanLyNhaHang_UNETI02_TI17A2ND.sln
QuanLyNhaHang_UNETI02_TI17A2ND/
 ├─ Models/            Entity: TaiKhoan, LoaiBan, BanAn, MonAn, KhachHang, PhieuDatBan
 ├─ Models/ViewModels/ Các ViewModel (đăng nhập, đăng ký, đặt bàn, dashboard, danh sách bàn...)
 ├─ Data/              ApplicationDbContext + DbInitializer (seed dữ liệu mẫu)
 ├─ Common/            Hằng số trạng thái, PagedList, các Attribute phân quyền theo Session
 ├─ Controllers/       TaiKhoan, LoaiBan, BanAn, MonAn, KhachHang, PhieuDatBan, ThongKe, Home
 ├─ Views/              Razor View tương ứng từng Controller
 └─ wwwroot/            CSS/JS (Bootstrap/jQuery được nhúng qua CDN trong _Layout.cshtml)
```

## 3. Chạy dự án lần đầu
1. Mở `QuanLyNhaHang_UNETI02_TI17A2ND.sln` bằng Visual Studio (hoặc `cd` vào thư mục project rồi dùng CLI).
2. Kiểm tra/sửa chuỗi kết nối trong `appsettings.json` (mặc định dùng LocalDB):
   ```
   Server=(localdb)\mssqllocaldb;Database=QuanLyNhaHang_UNETI02_TI17A2ND_Db;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
   ```
3. Tạo Migration đầu tiên (môi trường build này không có sẵn .NET SDK nên Migration **chưa được sinh sẵn** —  cần chạy lệnh sau trên máy có cài .NET 10 SDK):
   ```
   cd QuanLyNhaHang_UNETI02_TI17A2ND
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   (Nếu chưa có công cụ `dotnet-ef`: `dotnet tool install --global dotnet-ef`)
4. Chạy chương trình:
   ```
   dotnet run
   ```
   Hoặc nhấn F5 trong Visual Studio. Ứng dụng sẽ tự động gọi `Database.Migrate()` và sinh dữ liệu mẫu (`DbInitializer.Seed`) khi khởi động lần đầu.

## 4. Tài khoản kiểm thử (được seed sẵn)
| Vai trò     | Tên đăng nhập     | Mật khẩu | Ghi chú                    |
|-------------|-------------------|----------|----------------------------|
| Admin       | admin             | admin123 | Quản trị viên 1            |
| Admin       | admin2            | admin123 | Quản trị viên 2            |
| Khách hàng  | khachhang01..14   | 123456   | Tài khoản hoạt động        |
| Khách hàng  | khachhang15       | 123456   | Tài khoản bị khóa (kiểm thử)|

## 5. Dữ liệu mẫu được sinh tự động
- 04 loại bàn, 20 bàn ăn (đủ để phân trang), 20 món ăn, 15 khách hàng, 2 tài khoản Admin, 24 phiếu đặt bàn
  trải đều các trạng thái Chờ xác nhận / Đã xác nhận / Đã sử dụng / Đã hủy.

