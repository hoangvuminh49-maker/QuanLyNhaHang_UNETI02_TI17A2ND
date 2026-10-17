using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_UNETI02_TI17A2ND.Models;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<LoaiBan>()
                .HasIndex(l => l.TenLoaiBan)
                .IsUnique();

            modelBuilder.Entity<BanAn>()
                .HasIndex(b => b.TenBan)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasOne(k => k.TaiKhoan)
                .WithOne(t => t.KhachHang)
                .HasForeignKey<KhachHang>(k => k.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BanAn>()
                .HasOne(b => b.LoaiBan)
                .WithMany(l => l.BanAns)
                .HasForeignKey(b => b.MaLoaiBan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuDatBan>()
                .HasOne(p => p.KhachHang)
                .WithMany(k => k.PhieuDatBans)
                .HasForeignKey(p => p.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhieuDatBan>()
                .HasOne(p => p.BanAn)
                .WithMany(b => b.PhieuDatBans)
                .HasForeignKey(p => p.MaBan)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
