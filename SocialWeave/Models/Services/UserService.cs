using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System.Data;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using SocialWeave.Models.AbstractClasses;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service that manages operations related to users within the application context.
    /// </summary>
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IConfiguration _configuration;
        private readonly PostService _postService;

        public UserService(ApplicationDbContext context,
               IActionContextAccessor actionContextAccessor,
               IHttpContextAccessor httpContextAccessor,
               IUrlHelperFactory urlHelperFactory,
               IConfiguration configuration,
               PostService postService)
        {
            _context = context;
            _actionContextAccessor = actionContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _configuration = configuration;
            _postService = postService;
        }

        /// <summary>
        /// Finds a user by email asynchronously.'
        /// </summary>
        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        /// <summary>
        /// Finds a user by name asynchronously.
        /// </summary>
        public async Task<User> FindUserByNameAsync(string name)
        {
            User user = await _context.Users
                                      .Include(x => x.Posts)
                                      .Include(x => x.Admirations)
                                      .FirstOrDefaultAsync(x => x.Name == name);

            await _postService.CompletePostAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> ReturnAdmiredFromUserAsync(User user)
        {
            var users = await _context.Admirations
                                      .Include(x => x.UserAdmired)
                                      .Include(x => x.UserAdmired.Posts)
                                      .Where(x => x.UserAdmiredId != user.Id && x.UserAdmirerId == user.Id)
                                      .Select(x => x.UserAdmired)
                                      .ToListAsync();

            return users;
        }

        public async Task<bool> CheckNameExistsAsync(string name) 
        {
            return await _context.Users.AnyAsync(x => x.Name == name);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        /// <summary>
        /// Finds a user by id asynchronously.
        /// </summary>
        public async Task<User> FindUserByIdAsync(Guid id)
        {
            return await _context.Users
                                 .Include(x => x.Posts)
                                 .Include(x => x.Admirations)
                                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Admiration> FindAdmirationByUserIdAsync(User userAdmired, User userAdmirer)
        {
            return await _context.Admirations
                                 .FirstOrDefaultAsync(x => x.UserAdmiredId == userAdmired.Id && x.UserAdmirerId == userAdmirer.Id);
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
            if (userDb == null)
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(userVM.Password, userDb.Password);
        }

        /// <summary>
        /// Creates a new user asynchronously in the database.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <exception cref="UserException">Exception thrown if there is an error creating the user.</exception>
        /// <exception cref="IntegrityException">Exception thrown if a concurrency error occurs in the database.</exception>
        public async Task CreateUserAsync(UserCreateViewModel userCreateVM)
        {
            try
            {
                if (await CheckEmailExistsAsync(userCreateVM.Email)) 
                {
                    throw new UserException("Existing email");
                }

                if (await CheckNameExistsAsync(userCreateVM.Name)) 
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
            }
            catch (DBConcurrencyException e)
            {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task AddPictureProfileAsync(string imageBytes, User user)
        {
            if (imageBytes == null || user == null)
            {
                throw new UserException("An error ocurred with reference null!");
            }

            var byteStrings = imageBytes.Split(',');
            var bytes = byteStrings.Select(s => byte.Parse(s)).ToArray();
            user.PictureProfile = bytes;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeDescriptionAsync(string description, User user)
        {
            if (description == null || user == null)
            {
                throw new UserException("An error ocurred with reference null!");
            }

            user.Description = description;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAdmirationAsync(User userAdmirer, User userAdmired) 
        {
            if(userAdmired == null || userAdmirer == null) 
            {
                throw new UserException("An error ocurred, null reference!");
            }

            Admiration admirationToRemove = await FindAdmirationByUserIdAsync(userAdmired, userAdmirer);
            _context.Admirations.Remove(admirationToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task AddAdmirationAsync(User userAdmirer, User userAdmired)
        {
            if (userAdmired == null || userAdmirer == null)
            {
                throw new UserException("An error ocurred, null reference!");
            }

            Admiration admiration = new Admiration()
            {
                Id = new Guid(),
                UserAdmiredId = userAdmired.Id,
                UserAdmired = userAdmired,
                UserAdmirerId = userAdmirer.Id,
                UserAdmirer = userAdmirer
            };

            await _context.Admirations.AddAsync(admiration);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> FindAdmirersOfUserAsync(User user) 
        {
            if (user == null) throw new UserException("User null!");

            var userAdmirers = await _context.Admirations
                                             .Where(x => x.UserAdmirerId != user.Id && x.UserAdmiredId == user.Id)
                                             .Include(x => x.UserAdmirer.Posts)
                                             .Include(x => x.UserAdmirer.Admirations)
                                             .Select(x => x.UserAdmirer)
                                             .ToListAsync();
            return userAdmirers;
        }

        public async Task<IEnumerable<User>> FindAdmiredByUserAsnyc(User user) 
        {
            if (user == null) throw new UserException("User null!");

            var userAdmired = await _context.Admirations.Where(x => x.UserAdmirerId == user.Id && x.UserAdmiredId != user.Id)
                                            .Select(x => x.UserAdmired)
                                            .ToListAsync();
            return userAdmired;
        }

        public async Task<string> FormatNumberAsync(int count)
        {
            if (count < 999)
            {
                return count.ToString();
            }
            else if (count < 9999)
            {
                return (count / 1000.0).ToString("0.#") + " K";
            }
            else if (count < 99999)
            {
                return (count / 1000.0).ToString("0.##") + " K";
            }
            else if (count < 999999)
            {
                return (count / 1000.0).ToString("0.#") + " K";
            }
            else
            {
                return (count / 1000000.0).ToString("0.#") + " M";
            }
        }

        public async Task<string> CountAdmiredAsync(User user)
        {
            if (user == null)
            {
                throw new UserException("User is null!");
            }

            var amountOfAdmired = await _context.Admirations
                .Where(x => x.UserAdmirerId == user.Id)
                .CountAsync();

            return await FormatNumberAsync(amountOfAdmired);
        }

        public async Task<string> CountAdmirersAsync(User user)
        {
            if (user == null)
            {
                throw new UserException("User is null!");
            }

            var amountOfAdmirers = await _context.Admirations
                .Where(x => x.UserAdmirerId != user.Id && x.UserAdmiredId == user.Id)
                .CountAsync();

            return await FormatNumberAsync(amountOfAdmirers);
        }


        #region Send Email
        /// <summary>
        /// Sends a password reset email to the user with the provided email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>True if the password reset email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            // Find the user by email
            var user = await FindUserByEmailAsync(email);

            // If the user doesn't exist, return false
            if (user == null)
            {
                return false;
            }

            // Generate a password reset token manually
            var token = Guid.NewGuid().ToString();

            // Set the password reset token for the user
            await SetPasswordResetTokenAsync(user, token);

            // Generate the callback URL for resetting the password
            var callbackUrl = GeneratePasswordResetCallbackUrl(user.Email, token);

            // ... (código de envio de e-mail, conforme fornecido anteriormente)

            return true;
        }

        // This method generates the callback URL for resetting the password
        private string GeneratePasswordResetCallbackUrl(string email, string token)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.Action("ResetPassword", "User", new { token = token, email = email }, protocol: _httpContextAccessor.HttpContext.Request.Scheme);
        }


        /// <summary>
        /// Sets the password reset token for the user.
        /// </summary>
        /// <param name="user">The user for whom the token is being set.</param>
        /// <param name="token">The password reset token to be set.</param>
        private async Task SetPasswordResetTokenAsync(User user, string token)
        {
            user.ResetToken = token;
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}



