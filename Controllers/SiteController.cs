using Microsoft.AspNetCore.Mvc;

namespace WebServer.Controllers
{
    public class SiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
