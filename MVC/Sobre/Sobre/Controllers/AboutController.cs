using Microsoft.AspNetCore.Mvc;

namespace Sobre.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
