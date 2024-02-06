using Microsoft.AspNetCore.Mvc;

namespace SocialWeave.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
