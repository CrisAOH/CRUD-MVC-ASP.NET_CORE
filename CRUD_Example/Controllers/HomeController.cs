using Microsoft.AspNetCore.Mvc;

namespace CRUD_Example.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
