using Microsoft.AspNetCore.Mvc;

namespace SocialWeave.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult ShowNotifications()
        {
            return View();
        }
    }
}
