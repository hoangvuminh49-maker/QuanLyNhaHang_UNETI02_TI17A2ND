using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHang.Controllers
{
    public class HomeController : Controller
    {
        // GET: /  hoặc /Home/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}
