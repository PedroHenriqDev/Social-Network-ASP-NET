using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using System.Runtime.InteropServices;

namespace SocialWeave.Models.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task AddAdmirationRelatedNotificationAsync(User userAdmired, User userAdmirer) 
        {
            if(userAdmired == null || userAdmirer == null) 
            {
                throw new NotificationException("An brutal error ocurred in creation of notifcation!");
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
    } 
}
