using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Helpers;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;

namespace SocialWeave.Controllers
{
    /// <summary>
    /// Controller responsible for managing notifications.
    /// </summary>
    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class NotificationController : Controller
    {
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly NotificationHelper _notificationHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="userService">The service for handling user-related operations.</param>
        /// <param name="notificationService">The service for handling notification-related operations.</param>
        /// <param name="notificationHelper">The helper for managing notification-related functionalities.</param>
        public NotificationController(UserService userService,
            NotificationService notificationService,
            NotificationHelper notificationHelper)
        {
            _userService = userService;
            _notificationService = notificationService;
            _notificationHelper = notificationHelper;
        }

        /// <summary>
        /// Displays the notifications for the current user.
        /// </summary>
        /// <returns>The view displaying the notifications.</returns>
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

        /// <summary>
        /// Checks if the current user has notifications.
        /// </summary>
        /// <returns>A JSON result indicating whether the user has notifications.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult HasNotifications()
        {
            bool hasNotifications = _notificationHelper.HasNotifications;
            return Json(hasNotifications);
        }
    }
}
