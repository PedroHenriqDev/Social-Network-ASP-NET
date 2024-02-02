using Microsoft.AspNetCore.Mvc;

namespace SocialWeave.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
