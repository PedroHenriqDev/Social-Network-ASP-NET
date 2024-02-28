using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service class for validating user credentials during login.
    /// </summary>
    public class LoginService
    {
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        /// <param name="userService">The user service for retrieving user information.</param>
        public LoginService(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Validates user credentials asynchronously.
        /// </summary>
        /// <param name="userVM">The user view model containing the credentials to validate.</param>
        /// <returns>True if the credentials are valid; otherwise, false.</returns>
        public async Task<bool> ValidateUserCredentialsAsync(UserViewModel userVM)
        {
            // Find the user in the database by email
            User userDb = await _userService.FindUserByEmailAsync(userVM.Email);
            if (userDb == null)
            {
                // User not found, return false
                return false;
            }

            // Verify the hashed password against the stored hashed password in the database
            return BCrypt.Net.BCrypt.Verify(userVM.Password, userDb.Password);
        }
    }
}
