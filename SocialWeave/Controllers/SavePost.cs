using Microsoft.AspNetCore.Mvc;

namespace SocialWeave.Controllers
{

    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class SavePost : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
