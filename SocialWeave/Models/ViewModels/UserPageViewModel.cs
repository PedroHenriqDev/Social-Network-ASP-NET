using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.ConcreteClasses;
using System.Security.Cryptography.X509Certificates;

namespace SocialWeave.Models.ViewModels
{
    public class UserPageViewModel
    {
        private readonly ApplicationDbContext _context;
        public User User { get; set; }
        public string CountAdmired { get; set; }
        public string CountAdmirer {get; set;}
        public IEnumerable<Comment> Comments { get; set;}

        public UserPageViewModel(ApplicationDbContext context, User user, string countAdmired, string countAdmirer, IEnumerable<Comment> comments)
        {
            _context = context;
            User = user;
            CountAdmired = countAdmired;
            CountAdmirer = countAdmirer;
            Comments = comments;
        }

        public async Task<bool> HaveAdmiredAsync(string nameUser, string nameCurrentUser)
        {
            if (nameUser == null || nameCurrentUser == null)
            {
                return false;
            }

            return await _context.Admirations.AnyAsync(x => x.UserAdmired.Name == nameUser 
            && x.UserAdmirer.Name == nameCurrentUser);
        }
    }
}
