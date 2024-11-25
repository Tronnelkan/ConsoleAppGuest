using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Web.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Message"] = $"Вітаємо, {User.Identity.Name}!";
            }
            else
            {
                ViewData["Message"] = "Будь ласка, увійдіть у систему.";
            }

            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
