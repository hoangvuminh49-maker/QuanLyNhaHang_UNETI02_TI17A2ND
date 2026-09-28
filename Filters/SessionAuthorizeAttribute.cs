using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Filters
{
    // Attribute kiểm tra đăng nhập + phân quyền dựa trên Session.
    // Dùng thay cho [Authorize] của ASP.NET Core Identity vì đề bài không bắt buộc dùng Identity.
    //
    // Cách dùng:
    //   [SessionAuthorize]                       -> chỉ cần đã đăng nhập (không phân biệt vai trò)
    //   [SessionAuthorize(VaiTro.Admin)]         -> chỉ Admin được truy cập
    //   [SessionAuthorize(VaiTro.KhachHang)]     -> chỉ Khách hàng được truy cập
    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly VaiTro[]? _vaiTroChoPhep;

        public SessionAuthorizeAttribute(params VaiTro[] vaiTroChoPhep)
        {
            _vaiTroChoPhep = vaiTroChoPhep.Length > 0 ? vaiTroChoPhep : null;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");

            // Chưa đăng nhập -> chuyển về trang đăng nhập
            if (maTaiKhoan == null)
            {
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
                return;
            }

            // Đã đăng nhập nhưng không đúng vai trò được phép -> từ chối truy cập
            if (_vaiTroChoPhep != null)
            {
                var vaiTroStr = session.GetString("VaiTro");
                if (vaiTroStr == null || !Enum.TryParse<VaiTro>(vaiTroStr, out var vaiTro)
                    || !_vaiTroChoPhep.Contains(vaiTro))
                {
                    context.Result = new RedirectToActionResult("TruyCapBiTuChoi", "TaiKhoan", null);
                }
            }
        }
    }
}
