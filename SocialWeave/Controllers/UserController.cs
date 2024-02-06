using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using SocialWeave.Exceptions;
using System.Diagnostics;
using System.Security.Claims;

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
                    new Claim(ClaimTypes.NameIdentifier, user.Name)
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
        public async Task<IActionResult> Register(User user)
        {                     
            // Remove sensitive information from the model's ModelState
            ModelState.Remove(nameof(user.Salt));
            ModelState.Remove(nameof(user.Posts));
            ModelState.Remove(nameof(user.ResetToken));
            ModelState.Remove(nameof(user.DateCreation));
            // Checks if the model is valid and attempts to create the user
            if (ModelState.IsValid && await _userService.CreateUserAsync(user))
            {
                // Redirects to the Login action if registration is successful
                return RedirectToAction(nameof(Login));
            }

            // Returns the registration view if there are model errors or if user creation fails
            return View();
        }

        public async Task<IActionResult> LogoutGet() 
        {
            return View(await _userService.FindUserByEmailAsync(User.Identity.Name));
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
