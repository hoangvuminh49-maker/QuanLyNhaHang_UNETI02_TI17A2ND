using QuanLyNhaHang_UNETI02_TI17A2ND.Common;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Data
{
    /// <summary>
    /// Sinh dữ liệu mẫu tối thiểu theo yêu cầu đề bài:
    /// 04 loại bàn, 20 bàn ăn, 20 món ăn, 15 khách hàng, 02 tài khoản Admin,
    /// các tài khoản khách hàng tương ứng và 20-30 phiếu đặt bàn.
    /// </summary>
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.TaiKhoans.Any()) return; // Đã có dữ liệu thì không seed lại

            // ===== Tài khoản Admin =====
            var admin1 = new TaiKhoan { TenDangNhap = "admin", MatKhau = "admin123", HoTen = "Quản trị viên 1", Email = "admin1@quanlynhahang.vn", VaiTro = VaiTro.Admin, TrangThai = true };
            var admin2 = new TaiKhoan { TenDangNhap = "admin2", MatKhau = "admin123", HoTen = "Quản trị viên 2", Email = "admin2@quanlynhahang.vn", VaiTro = VaiTro.Admin, TrangThai = true };
            context.TaiKhoans.AddRange(admin1, admin2);
            context.SaveChanges();

            // ===== Loại bàn (04) =====
            var loaiBans = new List<LoaiBan>
            {
                new() { TenLoaiBan = "Bàn thường", MoTa = "Bàn phục vụ khách lẻ, gia đình nhỏ", TrangThai = true },
                new() { TenLoaiBan = "Bàn VIP", MoTa = "Bàn phòng riêng, phục vụ cao cấp", TrangThai = true },
                new() { TenLoaiBan = "Bàn ngoài trời", MoTa = "Bàn sân vườn, view thoáng mát", TrangThai = true },
                new() { TenLoaiBan = "Bàn tiệc", MoTa = "Bàn sức chứa lớn, phục vụ tiệc/đoàn khách", TrangThai = true },
            };
            context.LoaiBans.AddRange(loaiBans);
            context.SaveChanges();

            // ===== Bàn ăn (20) =====
            string[] viTriList = { "Tầng 1", "Tầng 2", "Sân vườn", "Ban công", "Phòng riêng" };
            var banAns = new List<BanAn>();
            for (int i = 1; i <= 20; i++)
            {
                var loaiBan = loaiBans[(i - 1) % loaiBans.Count];
                int soCho = loaiBan.TenLoaiBan == "Bàn tiệc" ? 12 : (loaiBan.TenLoaiBan == "Bàn VIP" ? 6 : 4);
                string trangThai = i % 7 == 0 ? TrangThaiBan.TamNgung : (i % 5 == 0 ? TrangThaiBan.DangSuDung : TrangThaiBan.SanSang);

                banAns.Add(new BanAn
                {
                    TenBan = $"Bàn {i:D2}",
                    MaLoaiBan = loaiBan.MaLoaiBan,
                    SoChoNgoi = soCho,
                    ViTri = viTriList[(i - 1) % viTriList.Length],
                    MoTa = $"Bàn số {i:D2} thuộc {loaiBan.TenLoaiBan}",
                    TrangThai = trangThai
                });
            }
            context.BanAns.AddRange(banAns);
            context.SaveChanges();

            // ===== Món ăn (20) =====
            (string ten, string loai, decimal gia)[] monMau =
            {
                ("Gỏi cuốn tôm thịt", "Khai vị", 45000),
                ("Nem rán", "Khai vị", 50000),
                ("Súp hải sản", "Khai vị", 55000),
                ("Salad rau trộn", "Khai vị", 40000),
                ("Cơm chiên hải sản", "Món chính", 75000),
                ("Cơm rang dưa bò", "Món chính", 65000),
                ("Phở bò tái", "Món chính", 60000),
                ("Bún chả Hà Nội", "Món chính", 65000),
                ("Cá kho tộ", "Món chính", 95000),
                ("Gà nướng mật ong", "Món chính", 120000),
                ("Bò lúc lắc", "Món chính", 135000),
                ("Tôm hấp bia", "Món chính", 150000),
                ("Mực xào sa tế", "Món chính", 110000),
                ("Lẩu thái hải sản", "Lẩu", 250000),
                ("Lẩu gà lá giang", "Lẩu", 220000),
                ("Nước cam ép", "Đồ uống", 25000),
                ("Trà đào cam sả", "Đồ uống", 30000),
                ("Sinh tố bơ", "Đồ uống", 35000),
                ("Chè khúc bạch", "Tráng miệng", 30000),
                ("Bánh flan", "Tráng miệng", 20000),
            };
            var monAns = new List<MonAn>();
            for (int i = 0; i < monMau.Length; i++)
            {
                monAns.Add(new MonAn
                {
                    TenMon = monMau[i].ten,
                    LoaiMon = monMau[i].loai,
                    DonGia = monMau[i].gia,
                    MoTa = $"Món {monMau[i].ten} - đặc sản nhà hàng",
                    TrangThai = (i % 9 != 0) // vài món ngừng phục vụ để kiểm thử
                });
            }
            context.MonAns.AddRange(monAns);
            context.SaveChanges();

            // ===== Khách hàng (15) + tài khoản tương ứng =====
            string[] hoList = { "Nguyễn Văn", "Trần Thị", "Lê Hoàng", "Phạm Minh", "Hoàng Thị", "Vũ Đức", "Đặng Thị", "Bùi Văn", "Đỗ Thị", "Ngô Minh" };
            string[] tenList = { "An", "Bình", "Chi", "Dũng", "Hà", "Hải", "Hùng", "Lan", "Linh", "Nam", "Ngọc", "Phong", "Quân", "Thảo", "Trang" };

            var khachHangs = new List<KhachHang>();
            for (int i = 1; i <= 15; i++)
            {
                var hoTen = $"{hoList[(i - 1) % hoList.Length]} {tenList[(i - 1) % tenList.Length]}";
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = $"khachhang{i:D2}",
                    MatKhau = "123456",
                    HoTen = hoTen,
                    Email = $"khachhang{i:D2}@gmail.com",
                    VaiTro = VaiTro.KhachHang,
                    TrangThai = i != 15 // để 1 tài khoản bị khóa phục vụ kiểm thử
                };
                context.TaiKhoans.Add(taiKhoan);
                context.SaveChanges();

                var khachHang = new KhachHang
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = hoTen,
                    NgaySinh = new DateTime(1990 + i, (i % 12) + 1, (i % 28) + 1),
                    GioiTinh = i % 2 == 0 ? "Nữ" : "Nam",
                    SoDienThoai = $"09{i:D8}",
                    Email = taiKhoan.Email,
                    DiaChi = $"Số {i} đường Nguyễn Trãi, Hà Nội",
                    NgayDangKy = DateTime.Now.AddDays(-i * 10),
                    TrangThai = taiKhoan.TrangThai
                };
                context.KhachHangs.Add(khachHang);
                context.SaveChanges();
                khachHangs.Add(khachHang);
            }

            // ===== Phiếu đặt bàn (24) =====
            var banPhucVu = banAns.Where(b => b.TrangThai != TrangThaiBan.TamNgung).ToList();
            string[] trangThaiPhieuMau = { TrangThaiPhieu.ChoXacNhan, TrangThaiPhieu.DaXacNhan, TrangThaiPhieu.DaSuDung, TrangThaiPhieu.DaHuy };
            var random = new Random(2026);
            var phieuDatBans = new List<PhieuDatBan>();

            for (int i = 1; i <= 24; i++)
            {
                var kh = khachHangs[(i - 1) % khachHangs.Count];
                var ban = banPhucVu[(i - 1) % banPhucVu.Count];
                var trangThai = trangThaiPhieuMau[(i - 1) % trangThaiPhieuMau.Length];
                int gioBd = 17 + (i % 5); // 17h -> 21h
                var ngaySuDung = DateTime.Today.AddDays((i % 10) - 3); // trải dài quanh ngày hiện tại

                phieuDatBans.Add(new PhieuDatBan
                {
                    MaKhachHang = kh.MaKhachHang,
                    MaBan = ban.MaBan,
                    NgayDat = DateTime.Now.AddDays(-random.Next(1, 15)),
                    NgaySuDung = ngaySuDung,
                    GioBatDau = new TimeSpan(gioBd, 0, 0),
                    GioKetThuc = new TimeSpan(gioBd + 2, 0, 0),
                    SoNguoi = Math.Min(ban.SoChoNgoi, 2 + (i % 4)),
                    GhiChu = i % 3 == 0 ? "Khách yêu cầu bàn yên tĩnh" : null,
                    TrangThai = trangThai
                });
            }
            context.PhieuDatBans.AddRange(phieuDatBans);
            context.SaveChanges();
        }
    }
}
