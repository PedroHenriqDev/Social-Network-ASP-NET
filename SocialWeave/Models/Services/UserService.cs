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

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service that manages operations related to users within the application context.
    /// </summary>
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context,
               UserManager<User> userManager,
               IActionContextAccessor actionContextAccessor,
               IHttpContextAccessor httpContextAccessor, 
               IUrlHelperFactory urlHelperFactory,
               IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _actionContextAccessor = actionContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _configuration = configuration;
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
            if(userDb == null) 
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
        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                if (await FindUserByEmailAsync(user.Email) != null)
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

        #region Send Email
        //OBS: It will only be possible to obtain one hosting application!
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

            // Generate a password reset token for the user
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Generate the callback URL for resetting the password
            var callbackUrl = GeneratePasswordResetCallbackUrl(user.Email, token);

            // Get the email address from the configuration
            var fromAddressString = _configuration["SmtpSettings:Email"];

            // Validate if the email address from the configuration is not null or empty
            if (string.IsNullOrEmpty(fromAddressString))
            {
                // Log or handle the error and return false
                return false;
            }

            // Create a new MailAddress for the sender
            var fromAddress = new MailAddress(fromAddressString, "SocialWeave");
            // Create a new MailAddress for the recipient
            var toAddress = new MailAddress(email);
            var subject = "Reset Your Password";
            var body = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";

            try
            {
                // Create a new SmtpClient with the SMTP settings from the configuration
                using (var smtp = new SmtpClient
                {
                    Host = _configuration["SmtpSettings:Host"],
                    Port = int.Parse(_configuration["SmtpSettings:Port"]),
                    EnableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, _configuration["SmtpSettings:Password"])
                })
                {
                    // Create a new MailMessage with the from address, to address, subject, and body
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        // Send the email
                        await smtp.SendMailAsync(message);
                    }
                }

                // If everything went well, return true
                return true;
            }
            catch (Exception ex)
            {
                // If something went wrong, log the exception and return false
                // Handle exceptions (log or rethrow, depending on your needs)
                return false;
            }
        }

        // This method generates the callback URL for resetting the password
        private string GeneratePasswordResetCallbackUrl(string email, string token)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.Action("ResetPassword", "User", new { token = token, email = email }, protocol: _httpContextAccessor.HttpContext.Request.Scheme);
        }

        #endregion  
    }
}


