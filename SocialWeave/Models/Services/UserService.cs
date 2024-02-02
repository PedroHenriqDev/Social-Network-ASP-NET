using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System.Data;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service that manages operations related to users within the application context.
    /// </summary>
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Finds a user by email asynchronously.
        /// </summary>
        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        /// <summary>
        /// Sets the encrypted password for a user using BCrypt.
        /// </summary>
        public void SetCryptPassword(User user)
        {
            user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);
        }

        /// <summary>
        /// Validates user credentials asynchronously, checking if the provided password matches the hashed password in the database.
        /// </summary>
        /// <param name="user">The user with the credentials to be validated.</param>
        /// <returns>True if credentials are valid, False otherwise.</returns>
        public async Task<bool> ValidateUserCredentialsAsync(UserViewModel userVM)
        {
            User userDb = await FindUserByEmailAsync(userVM.Email);
            return BCrypt.Net.BCrypt.Verify(userVM.Password, userDb.Password) && userDb != null;
        }

        /// <summary>
        /// Creates a new user asynchronously in the database.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <exception cref="UserException">Exception thrown if there is an error creating the user.</exception>
        /// <exception cref="IntegrityException">Exception thrown if a concurrency error occurs in the database.</exception>
        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                if(await FindUserByEmailAsync(user.Email) != null) 
                {
                    return false;
                }

                SetCryptPassword(user);
                user.Id = Guid.NewGuid();
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DBConcurrencyException e)
            {
                throw new IntegrityException(e.Message);
            }
        }
    }
}
