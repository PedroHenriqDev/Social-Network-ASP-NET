using SocialWeave.Models.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SocialWeave.Helpers
{
    /// <summary>
    /// Helper class for managing notifications.
    /// </summary>
    public class NotificationHelper
    {
        /// <summary>
        /// Gets or sets a value indicating whether there are notifications.
        /// </summary>
        public bool HasNotifications { get; set; } = false;

        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHelper"/> class.
        /// </summary>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="userService">The user service.</param>
        public NotificationHelper(NotificationService notificationService,
                                  IHttpContextAccessor httpContextAccessor,
                                  UserService userService)
        {
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        /// <summary>
        /// Sets the value indicating whether there are notifications asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SetHasNotificationAsync()
        {
            HasNotifications = await _notificationService.HasNotificationAsync(await _userService.FindUserByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name));
            SetHasNotification(HasNotifications);
        }

        /// <summary>
        /// Sets the value indicating whether there are notifications.
        /// </summary>
        /// <param name="hasNotification">A value indicating whether there are notifications.</param>
        public void SetHasNotification(bool hasNotification)
        {
            ISession session = _httpContextAccessor.HttpContext.Session;
            session.SetString("HasNotifications", hasNotification.ToString());
        }
    }
}
