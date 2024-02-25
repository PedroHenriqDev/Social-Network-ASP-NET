using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Helpers;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;

namespace SocialWeave.Controllers
{
    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class NotificationController : Controller
    {

        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly NotificationHelper _notificationHelper;

        public NotificationController(UserService userService, 
            NotificationService notificationService, 
            NotificationHelper notificationHelper) 
        {
            _userService = userService;
            _notificationService = notificationService;
            _notificationHelper = notificationHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ShowNotifications()
        {
            try
            {
                User currentUser = await _userService.FindUserByNameAsync(User.Identity.Name);
                var notifications = await _notificationService.FindNotificationsByUserAsync(currentUser);
                await _notificationHelper.SetHasNotificationAsync();
                return View(notifications);
            }
            catch (NotificationException ex)
            {
                Console.WriteLine(ex.Message);
                return View("Index", "Home");
            }
            catch (UserException) 
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult HasNotifications()
        {
            bool hasNotifications = _notificationHelper.HasNotifications;
            return Json(hasNotifications);
        }
    }
}
