using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
