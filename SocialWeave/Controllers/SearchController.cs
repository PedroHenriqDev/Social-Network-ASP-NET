using Microsoft.AspNetCore.Mvc;

namespace SocialWeave.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
