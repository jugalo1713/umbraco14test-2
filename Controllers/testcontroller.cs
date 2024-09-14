using Microsoft.AspNetCore.Mvc;

namespace umbraco14test_2.Controllers
{
    public class testcontroller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
