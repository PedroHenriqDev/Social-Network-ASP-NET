using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task AddAdmirationRelatedNotificationAsync(User userAdmired, User userAdmirer) 
        {
            if(userAdmired == null || userAdmirer == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                User = userAdmired,
                Date = DateTime.Now,
                WasSeen = false,
                Content = userAdmirer.Name + " admired you!"
            };

            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();
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
                Notification notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    User = user,
                    Date = DateTime.Now,
                    Content = user.Name + " posted something new!",
                    WasSeen = false
                };
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddNotificationRelatedLikeAsync(User userLike, User userPost)
        {
            if (userLike == null || userPost == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                User = userPost,
                Date = DateTime.Now,
                Content = userLike.Name + " liked your post",
                WasSeen = false
            };
            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationRelatedCommentAsync(User userComment, User userPost) 
        {
            if(userComment == null || userPost == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notification!");
            }

            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                User = userPost,
                Date = DateTime.Now,
                Content = userComment.Name + " commented on your post",
                WasSeen = false
            };
            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
    } 
}
