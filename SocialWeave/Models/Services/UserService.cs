using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
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
        private async Task<User> FindUserByEmailAsync(User user)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
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
        public async Task<bool> ValidateUserCredentialsAsync(User user)
        {
            User userDb = await FindUserByEmailAsync(user);
            SetCryptPassword(user);
            return BCrypt.Net.BCrypt.Verify(user.Password, userDb.Password) && userDb != null;
        }

        /// <summary>
        /// Creates a new user asynchronously in the database.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <exception cref="UserException">Exception thrown if there is an error creating the user.</exception>
        /// <exception cref="IntegrityException">Exception thrown if a concurrency error occurs in the database.</exception>
        public async Task CreateUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("Error in creating user!");
                }
                SetCryptPassword(user);
                user.Id = Guid.NewGuid();
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
