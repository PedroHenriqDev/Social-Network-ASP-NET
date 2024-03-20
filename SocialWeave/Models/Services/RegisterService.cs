using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System.Data;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service class for registering users in the database.
    /// </summary>
    public class RegisterService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly ILogger<RegisterService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterService"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="userService">The user service for checking user information.</param>
        public RegisterService(ApplicationDbContext context, UserService userService, ILogger<RegisterService> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new user asynchronously in the database.
        /// </summary>
        /// <param name="userCreateVM">The view model containing the user information.</param>
        /// <exception cref="UserException">Thrown if there is an error creating the user.</exception>
        /// <exception cref="IntegrityException">Thrown if a concurrency error occurs in the database.</exception>
        public async Task RegisterUserAsync(UserCreateViewModel userCreateVM)
        {
            try
            {
                if (await _userService.CheckEmailExistsAsync(userCreateVM.Email))
                {
                    throw new UserException("Existing email");
                }

                if (await _userService.CheckNameExistsAsync(userCreateVM.Name))
                {
                    throw new UserException("Existing name");
                }

                User user = new User()
                {
                    Name = userCreateVM.Name,
                    Email = userCreateVM.Email,
                    Password = userCreateVM.Password,
                    PhoneNumber = userCreateVM.PhoneNumber,
                    BirthDate = userCreateVM.BirthDate,
                    Id = new Guid(),
                };

                SetCryptPassword(user);
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User creation successful.");
            }
            catch (DBConcurrencyException ex)
            {
                throw new IntegrityException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the encrypted password for a user using BCrypt.
        /// </summary>
        /// <param name="user">The user for whom the password is to be set.</param>
        private void SetCryptPassword(User user)
        {
            user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);
        }

    }
}
