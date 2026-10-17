// Họ và tên: Vũ Minh Hoàng
// Mã sinh viên: 23203100015
// Nội dung thực hiện: Các Attribute kiểm tra đăng nhập và phân quyền tại Controller (Module 1)
// Hệ thống dùng Session (không dùng Cookie Authentication/Identity) nên việc kiểm tra
// quyền truy cập được thực hiện bằng ActionFilter, áp dụng trực tiếp lên Controller/Action.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyNhaHang_UNETI02_TI17A2ND.Common
{
    /// <summary>
    /// Yêu cầu người dùng phải đăng nhập (không phân biệt vai trò).
    /// </summary>
    public class DaDangNhapAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            if (maTaiKhoan == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl });
            }
            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Chỉ cho phép tài khoản có VaiTro = Admin truy cập chức năng.
    /// Không chỉ ẩn nút/menu trên View mà bắt buộc kiểm tra tại Controller theo đúng yêu cầu đề bài.
    /// </summary>
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTro = context.HttpContext.Session.GetString(SessionKeys.VaiTro);
            if (vaiTro == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl });
            }
            else if (vaiTro != VaiTro.Admin)
            {
                context.Result = new ViewResult { ViewName = "~/Views/Shared/KhongDuQuyen.cshtml" };
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Chỉ cho phép tài khoản có VaiTro = KhachHang truy cập chức năng.
    /// </summary>
    public class KhachHangOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTro = context.HttpContext.Session.GetString(SessionKeys.VaiTro);
            if (vaiTro == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl });
            }
            else if (vaiTro != VaiTro.KhachHang)
            {
                context.Result = new ViewResult { ViewName = "~/Views/Shared/KhongDuQuyen.cshtml" };
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            base.OnActionExecuting(context);
        }
    }
}
