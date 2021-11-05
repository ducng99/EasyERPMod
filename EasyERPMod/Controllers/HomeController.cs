using Microsoft.AspNetCore.Mvc;

namespace EasyERPModV2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}