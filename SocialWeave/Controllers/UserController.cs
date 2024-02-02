using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
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
        public async Task<IActionResult> Login(User user)
        {
            ModelState.Remove(nameof(user.Salt));
            ModelState.Remove(nameof(user.PhoneNumber));
            ModelState.Remove(nameof(user.BirthDate));
            ModelState.Remove(nameof(user.Name));
            if (ModelState.IsValid && await _userService.ValidateUserCredentialsAsync(user))
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Email)
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
            return View(user);
        }

        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user) 
        {
            ModelState.Remove(nameof(user.Salt));
            ModelState.Remove(nameof(user.Posts));
            if (ModelState.IsValid) 
            {
                await _userService.CreateUserAsync(user);
                return RedirectToAction(nameof(Login));
            }
            return View();
        }

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
