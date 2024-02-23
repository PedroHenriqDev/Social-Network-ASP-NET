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
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public NotificationService(ApplicationDbContext context, UserService userService) 
        {
            _context = context;
            _userService = userService;
        }

        public async Task AddAdmirationRelatedNotificationAsync(User userAdmirer, User userAdmired) 
        {
            if(userAdmired == null || userAdmirer == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            if (userAdmired.Id != userAdmirer.Id)
            {
                Notification notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    User = userAdmired,
                    InvolvedUser = userAdmirer,
                    Date = DateTime.Now,
                    WasSeen = false,
                    Content = userAdmirer.Name + " admired you!"
                };

                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddNotificationRelatedToPostAsync(User userWhoPosted) 
        {
            if(userWhoPosted == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            var users = await _userService.FindAdmirersOfUserAsync(userWhoPosted);

            foreach(var user in users) 
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
                }
            }
        }

        public async Task AddNotificationRelatedLikeAsync(User userLike, User userPost)
        {
            if (userLike == null || userPost == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            if (userLike.Id != userPost.Id)
            {

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
            }
            }

            public async Task AddNotificationRelatedCommentAsync(User userComment, User userPost) 
        {
            if(userComment == null || userPost == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            if (userComment.Id != userPost.Id)
            {
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
            }
        }

        public async Task<IEnumerable<Notification>> FindNotificationsByUserAsync(User user) 
        {
            if (user == null)
            {
                throw new NotificationException("An brutal error ocurred in find user!");
            }
            var notifications = await _context.Notifications.Include(n => n.InvolvedUser)
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.Date)
                .ToListAsync();

            await ChangeStatusOfNoticationAsync(notifications);

            var filteredNotifications = await FilterNotificationByDateAsync(notifications);
            return filteredNotifications;
        }

        public async Task<bool> HasNotificationAsync(User user) 
        {
            if(user == null) 
            {
                return false;
            }

            return await _context.Notifications.AnyAsync(n => n.UserId == user.Id && !n.WasSeen);
        }

        private async Task ChangeStatusOfNoticationAsync(IEnumerable<Notification> notifications)
        {
            if (notifications == null)
            {
                throw new NotificationException("No notifications provided for status change.");
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    foreach (var notification in notifications)
                    {
                        notification.WasSeen = true;
                    }

                    _context.Notifications.UpdateRange(notifications);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new NotificationException("Error occurred while changing notification status.");
            }
        }

        private async Task<IEnumerable<Notification>> FilterNotificationByDateAsync(IEnumerable<Notification> notifications) 
        {
            if(!notifications.Any() || notifications == null) 
            {
                return notifications;
            }

            var notificationsToRemove = notifications
                .Where(n => n.WasSeen && (DateTime.Now - n.Date)
                .Days > 15).ToList();

            if(notificationsToRemove.Any() ) 
            {
                _context.Notifications.RemoveRange(notificationsToRemove);
                await _context.SaveChangesAsync();
            }

            return notifications.Except(notificationsToRemove);
        }

    }
}
