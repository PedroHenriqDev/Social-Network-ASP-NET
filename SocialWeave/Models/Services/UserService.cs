using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Constructor for UserService.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="actionContextAccessor">Accessor for action context.</param>
        /// <param name="httpContextAccessor">Accessor for HTTP context.</param>
        /// <param name="urlHelperFactory">Factory for URL helper.</param>
        /// <param name="configuration">Configuration options.</param>
        /// <param name="postService">Service for managing posts.</param>
        /// <param name="logger">The logger to log information, warnings, and errors.</param>
        public UserService(ApplicationDbContext context,
               IActionContextAccessor actionContextAccessor,
               IHttpContextAccessor httpContextAccessor,
               IUrlHelperFactory urlHelperFactory,
               IConfiguration configuration,
               PostService postService,
               ILogger<UserService> logger)
        {
            _context = context;
            _actionContextAccessor = actionContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _configuration = configuration;
            _postService = postService;
            _logger = logger;
        }

        /// <summary>
        /// Finds a user by email asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user with the specified email, or null if not found.</returns>
        public async Task<User> FindUserByEmailAsync(string email)
        {
            try
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                _logger.LogInformation("User search for email successfully completed");
                return user;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred when searching for the user by email");
                throw;
            }
        }

        /// <summary>
        /// Finds a user by name asynchronously.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <returns>The user with the specified name, or null if not found.</returns>
        public async Task<User> FindUserByNameAsync(string name)
        {
            try
            {
                User user = await _context.Users
                                          .Include(x => x.Posts)
                                          .Include(x => x.Admirations)
                                          .Include(x => x.SavedPosts)
                                          .FirstOrDefaultAsync(x => x.Name == name);

                await _postService.CompletePostAsync(user);
                _logger.LogInformation("User search for name successfully completed");
                return user;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "An error occurred when searching for the user by name");
                throw;
            }
        }

        /// <summary>
        /// Retrieves users admired by the specified user.
        /// </summary>
        /// <param name="user">The user whose admirers are to be retrieved.</param>
        /// <returns>A collection of users admired by the specified user.</returns>
        public async Task<IEnumerable<User>> ReturnAdmiredFromUserAsync(User user)
        {
            try
            {

                var users = await _context.Admirations
                                          .Include(x => x.UserAdmired)
                                          .Include(x => x.UserAdmired.Posts)
                                          .Where(x => x.UserAdmiredId != user.Id && x.UserAdmirerId == user.Id)
                                          .Select(x => x.UserAdmired)
                                          .ToListAsync();

                _logger.LogInformation("Successfully returning admired users.");
                return users;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred in return admired users");
                throw;
            }
        }

        /// <summary>
        /// Checks if a user with the specified name already exists.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>True if a user with the specified name exists; otherwise, false.</returns>
        public async Task<bool> CheckNameExistsAsync(string name)
        {
            try
            {
                bool nameExist = await _context.Users.AnyAsync(x => x.Name == name);
                _logger.LogInformation("Checking whether the name exists successfully.");
                return nameExist;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred when checking if the name exists");
                throw;
            }
        }

        /// <summary>
        /// Checks if a user with the specified email already exists.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>True if a user with the specified email exists; otherwise, false.</returns>
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                bool emailExist = await _context.Users.AnyAsync(x => x.Email == email);
                _logger.LogInformation("Checking whether the email exists successfully.");
                return emailExist;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred when checking if the email exists");
                throw;
            }
        }

        /// <summary>
        /// Finds a user by id asynchronously.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user with the specified id, or null if not found.</returns>
        public async Task<User> FindUserByIdAsync(Guid id)
        {
            try
            {
                User user = await _context.Users
                                     .Include(x => x.Posts)
                                     .Include(x => x.Admirations)
                                     .FirstOrDefaultAsync(x => x.Id == id);

                _logger.LogInformation("User search by id successful");
                return user;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while the user search took place");
                throw;
            }
        }

        /// <summary>
        /// Finds an admiration record between two users asynchronously.
        /// </summary>
        /// <param name="userAdmired">The user admired.</param>
        /// <param name="userAdmirer">The user admirer.</param>
        /// <returns>The admiration record between the specified users, or null if not found.</returns>
        public async Task<Admiration> FindAdmirationByUserIdAsync(User userAdmired, User userAdmirer)
        {
            try
            {
                Admiration admiration = await _context.Admirations
                                     .FirstOrDefaultAsync(x => x.UserAdmiredId == userAdmired.Id && x.UserAdmirerId == userAdmirer.Id);

                _logger.LogInformation("Search for admiration by user id done successfully.");
                return admiration;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while the search for admiration for the user id happenede");
                throw;
            }
        }

        /// <summary>
        /// Changes the description of a user asynchronously.
        /// </summary>
        /// <param name="description">The new description for the user.</param>
        /// <param name="user">The user whose description is being changed.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task ChangeDescriptionAsync(string description, User user)
        {
            try
            {
                if (description == null || user == null)
                {
                    throw new UserException("An error occurred with reference null!");
                }

                user.Description = description;
                _context.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Description change made successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while in change description");
            }
        }

        /// <summary>
        /// Removes admiration from one user to another asynchronously.
        /// </summary>
        /// <param name="userAdmirer">The user who admired.</param>
        /// <param name="userAdmired">The user who was admired.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task RemoveAdmirationAsync(User userAdmirer, User userAdmired)
        {
            try
            {
                if (userAdmired == null || userAdmirer == null)
                {
                    throw new UserException("An error occurred, null reference!");
                }

                Admiration admirationToRemove = await FindAdmirationByUserIdAsync(userAdmired, userAdmirer);
                _context.Admirations.Remove(admirationToRemove);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Admiration deletion done successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocurred while the deletion of admiration happened");
            }
        }

        /// <summary>
        /// Adds admiration from one user to another asynchronously.
        /// </summary>
        /// <param name="userAdmirer">The user who admires.</param>
        /// <param name="userAdmired">The user who is admired.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task AddAdmirationAsync(User userAdmirer, User userAdmired)
        {
            try
            {
                if (userAdmired == null || userAdmirer == null)
                {
                    throw new UserException("An error occurred, null reference!");
                }

                Admiration admiration = new Admiration()
                {
                    Id = Guid.NewGuid(),
                    UserAdmiredId = userAdmired.Id,
                    UserAdmired = userAdmired,
                    UserAdmirerId = userAdmirer.Id,
                    UserAdmirer = userAdmirer
                };

                await _context.Admirations.AddAsync(admiration);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Admiration deletion done successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocurred while the deletion of admiration happened");
            }
        }

        /// <summary>
        /// Finds users who admire the provided user asynchronously.
        /// </summary>
        /// <param name="user">The user whose admirers are being found.</param>
        /// <returns>A Task representing the asynchronous operation, yielding a collection of users who admire the provided user.</returns>
        public async Task<IEnumerable<User>> FindAdmirersOfUserAsync(User user)
        {
            try
            {
                if (user == null) throw new UserException("User null!");

                var userAdmirers = await _context.Admirations
                                                 .Where(x => x.UserAdmirerId != user.Id && x.UserAdmiredId == user.Id)
                                                 .Include(x => x.UserAdmirer.Posts)
                                                 .Include(x => x.UserAdmirer.Admirations)
                                                 .Select(x => x.UserAdmirer)
                                                 .ToListAsync();

                _logger.LogInformation("Search for admirers done successfully");
                return userAdmirers;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred when searching for admirers");
                throw;
            }
        }

        /// <summary>
        /// Finds users whom the provided user admires asynchronously.
        /// </summary>
        /// <param name="user">The user whose admired users are being found.</param>
        /// <returns>A Task representing the asynchronous operation, yielding a collection of users whom the provided user admires.</returns>
        public async Task<IEnumerable<User>> FindAdmiredByUserAsnyc(User user)
        {
            try
            {
                if (user == null) throw new UserException("User null!");

                var userAdmired = await _context.Admirations
                                                .Where(x => x.UserAdmirerId == user.Id && x.UserAdmiredId != user.Id)
                                                .Select(x => x.UserAdmired)
                                                .ToListAsync();
                
                _logger.LogInformation("Search for admired done successfully");
                return userAdmired;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred when searching for admired");
                throw;
            }
        }

        /// <summary>
        /// Formats a number asynchronously.
        /// </summary>
        /// <param name="count">The number to be formatted.</param>
        /// <returns>A Task representing the asynchronous operation, yielding the formatted number.</returns>
        public async Task<string> FormatNumberAsync(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Count cannot be negative.");
            }

            if (count < 1000)
            {
                return count.ToString();
            }
            else if (count < 1000000)
            {
                double formattedCount;
                string suffix;

                if (count < 10000)
                {
                    formattedCount = count / 1000.0;
                    suffix = "K";
                }
                else
                {
                    formattedCount = count / 1000000.0;
                    suffix = "M";
                }

                return $"{formattedCount:0.##} {suffix}";
            }
            else
            {
                return $"{count / 1000000.0:0.#} M";
            }
        }

        /// <summary>
        /// Counts the number of users who admire the provided user asynchronously.
        /// </summary>
        /// <param name="user">The user whose admirers are being counted.</param>
        /// <returns>A Task representing the asynchronous operation, yielding the count of admirers.</returns>
        public async Task<string> CountAdmirersAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("User is null!");
                }

                var amountOfAdmirers = await _context.Admirations
                    .Where(x => x.UserAdmirerId != user.Id && x.UserAdmiredId == user.Id)
                    .CountAsync();

                _logger.LogInformation("Success in counting admirers.");
                return await FormatNumberAsync(amountOfAdmirers);
            }
            catch(Exception ex) 
            {
                _logger.LogError("Error ocurred while the count of admirers took place");
                throw;
            }
        }

        /// <summary>
        /// Counts the number of users admired by the provided user asynchronously.
        /// </summary>
        /// <param name="user">The user whose admired users are being counted.</param>
        /// <returns>A Task representing the asynchronous operation, yielding the count of admired users.</returns>
        public async Task<string> CountAdmiredAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("User is null!");
                }

                var amountOfAdmired = await _context.Admirations
                    .Where(x => x.UserAdmirerId == user.Id)
                    .CountAsync();

                _logger.LogInformation("Success in counting admired.");
                return await FormatNumberAsync(amountOfAdmired);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while the count of admired took place.");
                throw;
            }
        }

        #region Send Email
        /// <summary>
        /// Sends a password reset email to the user with the provided email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>True if the password reset email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
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

                var callbackUrl = GeneratePasswordResetCallbackUrl(user.Email, token);

                _logger.LogInformation("Email sent successfully.");
                return true;
            }
            catch(Exception ex) 
            {
                _logger.LogError("Error sending email");
                throw;
            }
        }

        // Generate the callback URL for resetting the password
        private string GeneratePasswordResetCallbackUrl(string email, string token)
        {
            try
            {
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                _logger.LogInformation("Callback generate successfully.");
                return urlHelper.Action("ResetPassword", "User", new { token = token, email = email }, protocol: _httpContextAccessor.HttpContext.Request.Scheme);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error generating the call back");
                throw;
            }
        }

        /// <summary>
        /// Sets the password reset token for the user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom the token is being set.</param>
        /// <param name="token">The password reset token to be set.</param>
        private async Task SetPasswordResetTokenAsync(User user, string token)
        {
            try
            {
                user.ResetToken = token;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Reset token generated successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error generating a reset token");
            }
        }
        #endregion
    }
}




