IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [LoaiBans] (
    [MaLoaiBan] int NOT NULL IDENTITY,
    [TenLoaiBan] nvarchar(100) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_LoaiBans] PRIMARY KEY ([MaLoaiBan])
);

CREATE TABLE [MonAns] (
    [MaMon] int NOT NULL IDENTITY,
    [TenMon] nvarchar(150) NOT NULL,
    [LoaiMon] nvarchar(50) NOT NULL,
    [DonGia] decimal(18,2) NOT NULL,
    [MoTa] nvarchar(max) NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_MonAns] PRIMARY KEY ([MaMon])
);

CREATE TABLE [TaiKhoans] (
    [MaTaiKhoan] int NOT NULL IDENTITY,
    [TenDangNhap] nvarchar(50) NOT NULL,
    [MatKhau] nvarchar(100) NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [VaiTro] nvarchar(20) NOT NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_TaiKhoans] PRIMARY KEY ([MaTaiKhoan])
);

CREATE TABLE [BanAns] (
    [MaBan] int NOT NULL IDENTITY,
    [TenBan] nvarchar(50) NOT NULL,
    [MaLoaiBan] int NOT NULL,
    [SoChoNgoi] int NOT NULL,
    [ViTri] nvarchar(150) NULL,
    [MoTa] nvarchar(max) NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_BanAns] PRIMARY KEY ([MaBan]),
    CONSTRAINT [FK_BanAns_LoaiBans_MaLoaiBan] FOREIGN KEY ([MaLoaiBan]) REFERENCES [LoaiBans] ([MaLoaiBan]) ON DELETE NO ACTION
);

CREATE TABLE [KhachHangs] (
    [MaKhachHang] int NOT NULL IDENTITY,
    [MaTaiKhoan] int NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [NgaySinh] datetime2 NULL,
    [GioiTinh] nvarchar(10) NULL,
    [SoDienThoai] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [DiaChi] nvarchar(250) NULL,
    [NgayDangKy] datetime2 NOT NULL,
    [TrangThai] bit NOT NULL,
    CONSTRAINT [PK_KhachHangs] PRIMARY KEY ([MaKhachHang]),
    CONSTRAINT [FK_KhachHangs_TaiKhoans_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoans] ([MaTaiKhoan]) ON DELETE CASCADE
);

CREATE TABLE [PhieuDatBans] (
    [MaPhieu] int NOT NULL IDENTITY,
    [MaKhachHang] int NOT NULL,
    [MaBan] int NOT NULL,
    [NgayDat] datetime2 NOT NULL,
    [NgaySuDung] datetime2 NOT NULL,
    [GioBatDau] time NOT NULL,
    [GioKetThuc] time NOT NULL,
    [SoNguoi] int NOT NULL,
    [GhiChu] nvarchar(500) NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_PhieuDatBans] PRIMARY KEY ([MaPhieu]),
    CONSTRAINT [FK_PhieuDatBans_BanAns_MaBan] FOREIGN KEY ([MaBan]) REFERENCES [BanAns] ([MaBan]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PhieuDatBans_KhachHangs_MaKhachHang] FOREIGN KEY ([MaKhachHang]) REFERENCES [KhachHangs] ([MaKhachHang]) ON DELETE NO ACTION
);

CREATE INDEX [IX_BanAns_MaLoaiBan] ON [BanAns] ([MaLoaiBan]);

CREATE UNIQUE INDEX [IX_BanAns_TenBan] ON [BanAns] ([TenBan]);

CREATE UNIQUE INDEX [IX_KhachHangs_MaTaiKhoan] ON [KhachHangs] ([MaTaiKhoan]);

CREATE UNIQUE INDEX [IX_LoaiBans_TenLoaiBan] ON [LoaiBans] ([TenLoaiBan]);

CREATE INDEX [IX_PhieuDatBans_MaBan] ON [PhieuDatBans] ([MaBan]);

CREATE INDEX [IX_PhieuDatBans_MaKhachHang] ON [PhieuDatBans] ([MaKhachHang]);

CREATE UNIQUE INDEX [IX_TaiKhoans_TenDangNhap] ON [TaiKhoans] ([TenDangNhap]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260923064406_InitialCreate', N'10.0.0');

COMMIT;
GO

