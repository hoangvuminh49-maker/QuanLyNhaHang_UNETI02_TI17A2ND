namespace QuanLyNhaHang.Models
{
    // Vai trò tài khoản trong hệ thống
    public enum VaiTro
    {
        Admin = 1,
        KhachHang = 2
    }

    // Trạng thái của bàn ăn
    public enum TrangThaiBan
    {
        SanSang = 1,        // Sẵn sàng
        DangSuDung = 2,     // Đang sử dụng
        TamNgungPhucVu = 3  // Tạm ngừng phục vụ
    }

    // Trạng thái của phiếu đặt bàn
    public enum TrangThaiPhieu
    {
        ChoXacNhan = 1,   // Chờ xác nhận
        DaXacNhan = 2,    // Đã xác nhận
        DaSuDung = 3,     // Đã sử dụng
        DaHuy = 4         // Đã hủy
    }
}
