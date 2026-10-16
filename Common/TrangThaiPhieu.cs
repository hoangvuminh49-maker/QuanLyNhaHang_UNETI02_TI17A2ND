// Họ và tên: Đoàn Văn Hiệp
// Mã sinh viên: 23274800003
// Nội dung thực hiện: Hằng số trạng thái phiếu đặt bàn (Module 3)
namespace QuanLyNhaHang_UNETI02_TI17A2ND.Common
{
    public static class TrangThaiPhieu
    {
        public const string ChoXacNhan = "ChoXacNhan";
        public const string DaXacNhan = "DaXacNhan";
        public const string DaSuDung = "DaSuDung";
        public const string DaHuy = "DaHuy";

        public static readonly Dictionary<string, string> HienThi = new()
        {
            { ChoXacNhan, "Chờ xác nhận" },
            { DaXacNhan, "Đã xác nhận" },
            { DaSuDung, "Đã sử dụng" },
            { DaHuy, "Đã hủy" }
        };

        public static string Ten(string ma) => HienThi.TryGetValue(ma, out var ten) ? ten : ma;
    }
}
