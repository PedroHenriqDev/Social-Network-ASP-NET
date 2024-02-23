using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;

namespace SocialWeave.Controllers
{
    public class NotificationController : Controller
    {

        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public NotificationController(UserService userService, NotificationService notificationService) 
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> ShowNotifications()
        {
            try
            {
                User currentUser = await _userService.FindUserByNameAsync(User.Identity.Name);
                var notifications = await _notificationService.FindNotificationsByUserAsync(currentUser);
                return View(notifications);
            }
            catch (NotificationException ex)
            {
                Console.WriteLine(ex.Message);
                return View("Index", "Home");
            }
        }
    }
}
