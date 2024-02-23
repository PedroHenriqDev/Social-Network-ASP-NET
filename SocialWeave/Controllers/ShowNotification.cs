using Microsoft.AspNetCore.Mvc;

namespace SocialWeave.Controllers
{
    public class ShowNotification : Controller
    {
        public IActionResult ShowNotifications()
        {
            return View();
        }
    }
}
