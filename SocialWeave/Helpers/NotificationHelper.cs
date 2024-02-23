using SocialWeave.Models.Services;
using System.Reflection.Metadata.Ecma335;

namespace SocialWeave.Helpers
{
    public class NotificationHelper
    {
        public bool HasNotifications { get; set; } = false;

        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationHelper(NotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            UserService userService)
        {
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task SetHasNotificationAsync()
        {
            HasNotifications = await _notificationService.HasNotificationAsync(await _userService.FindUserByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name));
            SetHasNotification(HasNotifications);
        }

        public void SetHasNotification(bool hasNotification)
        {
            ISession session = _httpContextAccessor.HttpContext.Session;
            session.SetString("HasNotifications", hasNotification.ToString());
        }
    }
}
