// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Hằng số trạng thái bàn ăn (Module 2)
namespace QuanLyNhaHang_UNETI02_TI17A2ND.Common
{
    public static class TrangThaiBan
    {
        public const string SanSang = "SanSang";
        public const string DangSuDung = "DangSuDung";
        public const string TamNgung = "TamNgung";

        public static readonly Dictionary<string, string> HienThi = new()
        {
            { SanSang, "Sẵn sàng" },
            { DangSuDung, "Đang sử dụng" },
            { TamNgung, "Tạm ngừng phục vụ" }
        };

        public static string Ten(string ma) => HienThi.TryGetValue(ma, out var ten) ? ten : ma;
    }
}
