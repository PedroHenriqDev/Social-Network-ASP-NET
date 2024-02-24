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
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly ProfilePictureService _profilePictureService;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(UserService userService,
            ApplicationDbContext context, 
            PostService postService, 
            NotificationService notificationService, 
            ProfilePictureService profilePictureService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _context = context;
            _postService = postService;
            _notificationService = notificationService;
            _profilePictureService = profilePictureService;
            _httpContextAccessor = httpContextAccessor;
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

                var claims = new List<Claim>();
                User user = await _userService.FindUserByEmailAsync(userVM.Email);
                string profilePictureUrl = await _profilePictureService.SaveProfilePictureAsync(user.PictureProfile, _httpContextAccessor.HttpContext.Request.PathBase);

                // Create claims for the authenticated user
                if (user.PictureProfile != null)
                {
                    claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("ProfilePictureUrl", profilePictureUrl)
                };
                }
                else 
                {
                    claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Name),
                };
                }

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
            TempData["InvalidUser"] = "Incorrect Email or Password, try another!";
            return View(userVM);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) 
            {
                return NotFound();
            }
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
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction(nameof(Login));
                }
                // Returns the registration view if there are model errors or if user creation fails
                return View(userCreateVM);
            }
            catch (UserException ex)
            {
                TempData["ErrorMessage"] = "Existing user or email try another";
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
                    await _userService.CountAdmiredAsync(user),
                    await _userService.CountAdmirersAsync(user),
                    await _postService.FindCommentsByUserAsync(user));
                    
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

                User user = await _userService.FindUserByNameAsync(name);

                UserPageViewModel userPageVM = new UserPageViewModel(_context,
                    user,
                    await _userService.CountAdmiredAsync(user),
                    await _userService.CountAdmirersAsync(user),
                    await _postService.FindCommentsByUserAsync(user));

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
                User userAdmirer = await _userService.FindUserByNameAsync(User.Identity.Name);
                User userAdmired = await _userService.FindUserByIdAsync(id);

                await _userService.AddAdmirationAsync(userAdmirer, userAdmired);
                await _notificationService.AddAdmirationRelatedNotificationAsync(userAdmirer, userAdmired);

                return RedirectToAction(nameof(UserPage));
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message});  
            }
            catch(NotificationException ex) 
            {
                return RedirectToAction("Index", "Home");
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
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); ;
        }
    }
}
