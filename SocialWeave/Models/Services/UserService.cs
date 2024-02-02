using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using System.Data;
using System.Runtime.InteropServices;

namespace SocialWeave.Models.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<User> FindUserByEmailAsync(User user)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        }

        public void SetCryptPassword(User user) 
        {
            user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);
        }

        public async Task<bool> ValidateUserCredentialsAsync(User user) 
        {
            User userDb = await FindUserByEmailAsync(user); 
            if (userDb == null) 
            {
                return false;
            }

            SetCryptPassword(user);
            return BCrypt.Net.BCrypt.Verify(user.Password, userDb.Password);
        }

        public async Task CreateUserAsync(User user) 
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("Error in create user!");
                }

                SetCryptPassword(user);
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (UserException e)
            {
                throw new UserException(e.Message);
            }
            catch (DBConcurrencyException e)
            {
                throw new IntegrityException(e.Message);
            }
        }

    }
}
