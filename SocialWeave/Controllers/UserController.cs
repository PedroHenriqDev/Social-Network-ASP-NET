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
using System.Reflection.Metadata.Ecma335;
using SocialWeave.Data;

namespace SocialWeave.Controllers
{
    /// <summary>
    /// Controller responsible for user-related actions, including authentication.
    /// </summary>
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public UserController(UserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
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
            catch (UserException ex)
            {
                return View(userCreateVM);
            }
            catch (IntegrityException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        public async Task<IActionResult> LogoutGet()
        {
            try
            {
                User user = await _userService.FindUserByNameAsync(User.Identity.Name);
                return View(user);
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new {message = ex.Message});
            }
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
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserPage()
        {

            try
            {
                if (Request.Method != "GET")
                {
                    return NotFound();
                }

                var user = await _userService.FindUserByNameAsync(User.Identity.Name);
                var posts = user.Posts.OrderBy(x => x.Date).ToList();

                UserPageViewModel userPageVM = new UserPageViewModel(_context,
                    user,
                    await _userService.CountAdmiredAsync(
                    await _userService.FindUserByNameAsync(user.Name)),
                    await _userService.CountAdmirersAsync(
                    await _userService.FindUserByNameAsync(user.Name)));
                    userPageVM.User.Posts = posts;

                return View(userPageVM);
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserPageWithParams(string name)
        {
            try
            {
                if (Request.Method != "GET")
                {
                    return NotFound();
                }

                UserPageViewModel userPageVM = new UserPageViewModel(_context,
                    await _userService.FindUserByNameAsync(name),
                    await _userService.CountAdmiredAsync(
                    await _userService.FindUserByNameAsync(name)),
                    await _userService.CountAdmirersAsync(
                    await _userService.FindUserByNameAsync(name)));

                return View("UserPage", userPageVM);
            }
            catch (UserException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });

            }
        }

        [HttpGet]
        public async Task<IActionResult> AddPictureProfile()
        {
            if (Request.Method != "GET")
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPictureProfile(string imageBytes)
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                await _userService.AddPictureProfileAsync(imageBytes, await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction(nameof(UserPage));
            }
            catch (UserException)
            {
                return View(imageBytes);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmiration(Guid id) 
        {
            if(Request.Method != "POST") 
            {

            }

            try 
            {
                await _userService.RemoveAdmirationAsync(await _userService.FindUserByNameAsync(User.Identity.Name), 
                    await _userService.FindUserByIdAsync(id));
                return RedirectToAction(nameof(UserPage));
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(UserPage), new {message = ex.Message});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmiration(Guid id) 
        {
            if(Request.Method != "POST") 
            {
                return NotFound();
            }

            try 
            {
                await _userService.AddAdmirationAsync(await _userService.FindUserByNameAsync(User.Identity.Name),
                    await _userService.FindUserByIdAsync(id));
                return RedirectToAction(nameof(UserPage));
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message});  
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowAdmirers(Guid id) 
        {
            if (Request.Method != "GET")
            {
                return NotFound();
            }
            try
            {
                ViewData["User"] = await _userService.FindUserByIdAsync(id);
                return View(await _userService.FindAdmirersOfUserAsync(await _userService.FindUserByIdAsync(id)));
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new {message = ex.Message});
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowAdmired(Guid id) 
        {
            if(Request.Method != "GET") 
            {
                return NotFound();
            }
            try
            {
                ViewData["User"] = await _userService.FindUserByIdAsync(id);
                return View(await _userService.FindAdmiredByUserAsnyc(await _userService.FindUserByIdAsync(id)));
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeDescription(string description)
        {
            try 
            {
                await _userService.ChangeDescriptionAsync(description, await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction(nameof(UserPage));
            }
            catch (UserException ex) 
            {
                return RedirectToAction(nameof(UserPage));
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
