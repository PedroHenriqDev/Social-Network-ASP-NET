using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using SocialWeave.Exceptions;
using System.Diagnostics;
using System.Security.Claims;
using System.Data;
using SocialWeave.Models.AbstractClasses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialWeave.Controllers
{
    /// <summary>
    /// Controller responsible for user-related actions, including authentication.
    /// </summary>
        public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated) 
            {
                return NotFound();
            }
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request for user login, authenticating the user and creating the necessary claims.
        /// </summary>
        /// <param name="user">The user model containing login credentials.</param>
        /// <returns>Redirects to the home page if login is successful; otherwise, redirects to the login page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
           
            if (ModelState.IsValid && await _userService.ValidateUserCredentialsAsync(userVM))
            {

                User user = await _userService.FindUserByEmailAsync(userVM.Email);
                // Create claims for the authenticated user
                 var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Name)
                };
                    
                // Create a ClaimsIdentity and set authentication properties
                var claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            return View(userVM);
        }

        public IActionResult Register() 
        {
            return View();
        }

        /// <summary>
        /// Handles HTTP POST requests for user registration.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserCreateViewModel userCreateVM)
        {
            // Remove sensitive information from the model's ModelState
            try
            {
                // Checks if the model is valid and attempts to create the user
                if (ModelState.IsValid)
                {
                    // Redirects to the Login action if registration is successful
                    await _userService.CreateUserAsync(userCreateVM);
                    return RedirectToAction(nameof(Login));
                }
                // Returns the registration view if there are model errors or if user creation fails
                return View();
            }
            catch(UserException ex) 
            {
                return View(userCreateVM);
            }
            catch(IntegrityException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        public async Task<IActionResult> LogoutGet() 
        {
            User user = await _userService.FindUserByNameAsync(User.Identity.Name);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost() 
        {
            try
            {
                if (Request.Method != "POST")
                {
                    throw new RequestException("An brutal error ocurred in this request!");
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }
            catch (RequestException ex) 
            {
                return RedirectToAction(nameof(Error), new {message = ex.Message});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConnection() 
        {
            if(Request.Method != "POST")
            {
                return NotFound();
            }

            return View();
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserPage() 
        {
            if (Request.Method != "Get") 
            {
                return NotFound();
            }
            return View();
            
        }

        #region Controller Send Email To Recovery Password
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request for the ForgotPassword view, allowing users to request a password reset email.
        /// </summary>
        /// <param name="model">The view model containing user input data.</param>
        /// <returns>If the email is successfully sent, redirects to the ForgotPasswordConfirmation view; otherwise, returns to the ForgotPassword view with an error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.SendPasswordResetEmailAsync(model.Email);
                if (result)
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                else
                {
                    ModelState.AddModelError("", "No is possible send the email!");
                }
            }

            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation() 
        {
            return View();
        }

        #endregion

        /// <summary>
        /// Displays the error view with the specified error message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <returns>The error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message }); ;
        }
    }
}
