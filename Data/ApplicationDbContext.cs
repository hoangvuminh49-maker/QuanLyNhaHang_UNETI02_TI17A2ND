using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public DbSet<LoaiBan> LoaiBans { get; set; } = null!;
        public DbSet<BanAn> BanAns { get; set; } = null!;
        public DbSet<MonAn> MonAns { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<PhieuDatBan> PhieuDatBans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----- TaiKhoan -----
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique(); // Tên đăng nhập không được trùng

            // ----- LoaiBan -----
            modelBuilder.Entity<LoaiBan>()
                .HasIndex(l => l.TenLoaiBan)
                .IsUnique(); // Tên loại bàn không được trùng

            // ----- LoaiBan (1) - (n) BanAn -----
            modelBuilder.Entity<BanAn>()
                .HasOne(b => b.LoaiBan)
                .WithMany(l => l.DanhSachBanAn)
                .HasForeignKey(b => b.MaLoaiBan)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa LoaiBan nếu còn BanAn liên quan

            modelBuilder.Entity<BanAn>()
                .HasIndex(b => b.TenBan)
                .IsUnique(); // Tên bàn không được trùng

            // ----- TaiKhoan (1) - (1) KhachHang -----
            modelBuilder.Entity<KhachHang>()
                .HasOne(k => k.TaiKhoan)
                .WithOne(t => t.KhachHang)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.MaTaiKhoan)
                .IsUnique();

            // ----- KhachHang (1) - (n) PhieuDatBan -----
            modelBuilder.Entity<PhieuDatBan>()
                .HasOne(p => p.KhachHang)
                .WithMany(k => k.DanhSachPhieuDatBan)
                .HasForeignKey(p => p.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- BanAn (1) - (n) PhieuDatBan -----
            modelBuilder.Entity<PhieuDatBan>()
                .HasOne(p => p.BanAn)
                .WithMany(b => b.DanhSachPhieuDatBan)
                .HasForeignKey(p => p.MaBan)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Dữ liệu khởi tạo: 1 tài khoản Admin mặc định -----
            // Seed dùng để nhóm có ngay tài khoản kiểm thử sau khi update-database.
            modelBuilder.Entity<TaiKhoan>().HasData(new TaiKhoan
            {
                MaTaiKhoan = 1,
                TenDangNhap = "admin",
                MatKhau = "123456", // Yêu cầu bắt buộc của đề không đòi hỏi mã hóa; có thể nâng cấp sau (mục 18)
                HoTen = "Quản trị viên",
                Email = "admin@nhahang.com",
                VaiTro = VaiTro.Admin,
                TrangThai = true
            });

            // ----- Dữ liệu khởi tạo: một số loại bàn mẫu -----
            modelBuilder.Entity<LoaiBan>().HasData(
                new LoaiBan { MaLoaiBan = 1, TenLoaiBan = "Bàn thường", MoTa = "Bàn tiêu chuẩn cho khách lẻ", TrangThai = true },
                new LoaiBan { MaLoaiBan = 2, TenLoaiBan = "Bàn VIP", MoTa = "Bàn phòng riêng cao cấp", TrangThai = true },
                new LoaiBan { MaLoaiBan = 3, TenLoaiBan = "Bàn ngoài trời", MoTa = "Bàn khu vực sân vườn", TrangThai = true },
                new LoaiBan { MaLoaiBan = 4, TenLoaiBan = "Bàn nhóm lớn", MoTa = "Bàn dành cho đoàn/tiệc đông người", TrangThai = true }
            );
        }
    }
}
