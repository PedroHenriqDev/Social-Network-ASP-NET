using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using System.Runtime.InteropServices;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// This service handles notifications related to user actions.
    /// </summary>
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly ILogger<NotificationService> _logger;

        /// <summary>
        /// Constructor for NotificationService.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="logger">The logger to log information, warnings, and errors.</param>
        public NotificationService(ApplicationDbContext context, UserService userService, ILogger<NotificationService> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a notification related to user admiration.
        /// </summary>
        /// <param name="userAdmirer">The user who admired.</param>
        /// <param name="userAdmired">The user who was admired.</param>
        public async Task AddAdmirationRelatedNotificationAsync(User userAdmirer, User userAdmired)
        {
            try
            {
                // Check if users are valid
                if (userAdmired == null || userAdmirer == null)
                {
                    throw new NotificationException("An error occurred while creating the notification.");
                }

                // Check if admirer and admired are not the same user
                if (userAdmired.Id != userAdmirer.Id)
                {
                    // Create notification
                    Notification notification = new Notification()
                    {
                        Id = Guid.NewGuid(),
                        User = userAdmired,
                        InvolvedUser = userAdmirer,
                        Date = DateTime.Now,
                        WasSeen = false,
                        Content = userAdmirer.Name + " admired you!"
                    };

                    // Save notification to database
                    await _context.AddAsync(notification);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Notification related admiration added successfully.");
                }

                _logger.LogWarning("Notification creation impossible, for user reasons");

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while creating notification related admiration");
                throw;
            }
        }

        /// <summary>
        /// Adds notifications related to a user's posted content.
        /// </summary>
        /// <param name="userWhoPosted">The user who posted content.</param>
        public async Task AddNotificationRelatedToPostAsync(User userWhoPosted)
        {
            try
            {
                // Check if user is valid
                if (userWhoPosted == null)
                {
                    throw new NotificationException("An error occurred while creating the notification.");
                }

                // Find users who admire the user who posted
                var users = await _userService.FindAdmirersOfUserAsync(userWhoPosted);

                // Create notifications for each admirer
                foreach (var user in users)
                {
                    if (user.Id != userWhoPosted.Id)
                    {
                        Notification notification = new Notification()
                        {
                            Id = Guid.NewGuid(),
                            User = user,
                            InvolvedUser = userWhoPosted,
                            Date = DateTime.Now,
                            Content = user.Name + " posted something new!",
                            WasSeen = false
                        };
                        await _context.AddAsync(notification);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Notification related to a new post added successfully.");
                    }
                    _logger.LogWarning("Notification creation impossible, for user reasons");
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while creating notification related to a new post");
                throw;
            }
        }

        /// <summary>
        /// Adds a notification related to a user liking a post.
        /// </summary>
        /// <param name="userLike">The user who liked the post.</param>
        /// <param name="userPost">The user who posted the content.</param>
        public async Task AddNotificationRelatedLikeAsync(User userLike, User userPost)
        {
            try
            {
                // Check if users are valid
                if (userLike == null || userPost == null)
                {
                    throw new NotificationException("An error occurred while creating the notification.");
                }

                // Check if liker and poster are not the same user
                if (userLike.Id != userPost.Id)
                {
                    // Create notification
                    Notification notification = new Notification()
                    {
                        Id = Guid.NewGuid(),
                        InvolvedUser = userLike,
                        User = userPost,
                        Date = DateTime.Now,
                        Content = userLike.Name + " liked your post",
                        WasSeen = false
                    };
                    await _context.AddAsync(notification);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Notification related to a new post added successfully.");
                }

                _logger.LogWarning("Notification creation impossible, for user reasons");

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while creating notification related like");
                throw;
            }
        }

        /// <summary>
        /// Adds a notification related to a user commenting on a post.
        /// </summary>
        /// <param name="userComment">The user who commented.</param>
        /// <param name="userPost">The user who posted the content.</param>
        public async Task AddNotificationRelatedCommentAsync(User userComment, User userPost)
        {
            try
            {
                // Check if users are valid
                if (userComment == null || userPost == null)
                {
                    throw new NotificationException("An error occurred while creating the notification.");
                }

                // Check if commenter and poster are not the same user
                if (userComment.Id != userPost.Id)
                {
                    // Create notification
                    Notification notification = new Notification()
                    {
                        Id = Guid.NewGuid(),
                        User = userPost,
                        InvolvedUser = userComment,
                        Date = DateTime.Now,
                        Content = userComment.Name + " commented on your post",
                        WasSeen = false
                    };
                    await _context.AddAsync(notification);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Notification related comment added successfully.");
                }

                _logger.LogWarning("Notification creation impossible, for user reasons");

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while creating notification related comment");
                throw;
            }
        }

        /// <summary>
        /// Finds notifications for a specific user.
        /// </summary>
        /// <param name="user">The user to find notifications for.</param>
        /// <returns>A collection of notifications.</returns>
        public async Task<IEnumerable<Notification>> FindNotificationsByUserAsync(User user)
        {
            try
            {
                // Check if user is valid
                if (user == null)
                {
                    throw new NotificationException("An error occurred while finding the user.");
                }

                // Retrieve notifications for the user
                var notifications = await _context.Notifications.Include(n => n.InvolvedUser)
                    .Where(n => n.UserId == user.Id)
                    .OrderByDescending(n => n.Date)
                    .ToListAsync();

                // Change status of notifications
                await ChangeStatusOfNoticationAsync(notifications);

                // Filter notifications by date
                var filteredNotifications = await FilterNotificationByDateAsync(notifications);
                _logger.LogInformation("Search for notifications by user completed successfully.");
                return filteredNotifications;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while the search was going on");
                throw;
            }
        }

        /// <summary>
        /// Checks if a user has any unread notifications.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user has unread notifications, otherwise false.</returns>
        public async Task<bool> HasNotificationAsync(User user)
        {
            try
            {
                // Check if user is valid
                if (user == null)
                {
                    return false;
                }

                // Check if the user has any unread notifications
                bool hasNotification = await _context.Notifications.AnyAsync(n => n.UserId == user.Id && !n.WasSeen);
                _logger.LogInformation("Checking whether notification has been made successfully.");
                return hasNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocurred while checking whether the notification was carried out");
                throw;
            }
        }

        // Changes the status of notifications to "seen"
        private async Task ChangeStatusOfNoticationAsync(IEnumerable<Notification> notifications)
        {
            // Check if notifications are provided
            if (notifications == null)
            {
                throw new NotificationException("No notifications provided for status change.");
            }

            try
            {
                // Begin transaction
                using (var transaction = _context.Database.BeginTransaction())
                {
                    // Mark notifications as "seen"
                    foreach (var notification in notifications)
                    {
                        notification.WasSeen = true;
                    }

                    // Update database with changes
                    _context.Notifications.UpdateRange(notifications);
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    transaction.Commit();
                    _logger.LogInformation("Notification status change made successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocurred while the notification status change occurred");
                throw new NotificationException("Error occurred while changing notification status.");
            }
        }

        // Filters notifications by date and removes old notifications
        private async Task<IEnumerable<Notification>> FilterNotificationByDateAsync(IEnumerable<Notification> notifications)
        {
            try
            {
                // Check if notifications exist
                if (!notifications.Any() || notifications == null)
                {
                    return notifications;
                }

                // Find notifications older than 15 days
                var notificationsToRemove = notifications
                    .Where(n => n.WasSeen && (DateTime.Now - n.Date)
                    .Days > 15).ToList();

                // Remove old notifications
                if (notificationsToRemove.Any())
                {
                    _context.Notifications.RemoveRange(notificationsToRemove);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Notifications filtered successfully");

                // Return remaining notifications
                return notifications.Except(notificationsToRemove);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while filter notifications");
                throw;
            }
        }
    }
}
