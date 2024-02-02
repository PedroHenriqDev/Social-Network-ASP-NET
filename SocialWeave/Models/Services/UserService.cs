using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateUserCredentialsAsync(User user)
        {
            User userDb = await FindUserByEmailAsync(user.Email);
            if (userDb == null) 
            {
                return false;
            }

            return true;
        }

        private async Task<User> FindUserByEmailAsync(string Email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == Email);
        }

        public CreateUser(User user) 
        {
            if(user == null) 
            {

            }
        }

    }
}
