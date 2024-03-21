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
using SocialWeave.Data;
using Microsoft.AspNetCore.Authorization;
using SocialWeave.Helpers;

namespace SocialWeave.Controllers
{
    /// <summary>
    /// Controller responsible for handling user-related actions, including authentication, registration, profile management, and more.
    /// </summary>
    public class UserController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly ProfilePictureService _profilePictureService;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotificationHelper _notificationHelper;
        private readonly RegisterService _registerService;
        private readonly LoginService _loginService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The service responsible for user-related operations.</param>
        /// <param name="context">The application database context.</param>
        /// <param name="postService">The service responsible for post-related operations.</param>
        /// <param name="notificationService">The service responsible for notification-related operations.</param>
        /// <param name="profilePictureService">The service responsible for profile picture-related operations.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="notificationHelper">The helper class for managing notifications.</param>
        /// <param name="registerService">The service responsible for user registration.</param>
        /// <param name="loginService">The service responsible for user access</param>
        public UserController(UserService userService,
            ApplicationDbContext context,
            PostService postService,
            NotificationService notificationService,
            ProfilePictureService profilePictureService,
            IHttpContextAccessor httpContextAccessor,
            NotificationHelper notificationHelper, RegisterService registerService, LoginService loginService)
        {
            _userService = userService;
            _context = context;
            _postService = postService;
            _notificationService = notificationService;
            _profilePictureService = profilePictureService;
            _httpContextAccessor = httpContextAccessor;
            _notificationHelper = notificationHelper;
            _registerService = registerService;
            _loginService = loginService;
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <returns>The login view.</returns>
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound(); // Redirect to not found page if user is already authenticated
            }
            return View();
        }

        /// <summary>
        /// Handles the POST request for user login.
        /// </summary>
        /// <param name="userVM">The user view model containing login credentials.</param>
        /// <returns>Redirects to the home page if login is successful; otherwise, returns to the login page with an error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel userVM)
        {
            if (ModelState.IsValid && await _loginService.ValidateUserCredentialsAsync(userVM))
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
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home"); // Redirect to home page after successful login
            }
            TempData["InvalidUser"] = "Incorrect Email or Password, try another!";
            return View(userVM); // Redirect back to login page with error message
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        /// <returns>The registration view.</returns>
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return NotFound(); // Redirect to not found page if user is already authenticated
            }
            return View();
        }

        /// <summary>
        /// Handles the POST request for user registration.
        /// </summary>
        /// <param name="userCreateVM">The user create view model containing registration data.</param>
        /// <returns>Redirects to the login page if registration is successful; otherwise, returns to the registration page with an error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserCreateViewModel userCreateVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _registerService.RegisterUserAsync(userCreateVM);
                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction(nameof(Login)); // Redirect to login page after successful registration
                }
                return View(userCreateVM); // Redirect back to registration page with error messages
            }
            catch (UserException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(userCreateVM); // Redirect back to registration page with error message
            }
            catch (IntegrityException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of database integrity error
            }
        }

        /// <summary>
        /// Displays the logout page.
        /// </summary>
        /// <returns>The logout view.</returns>
        [ServiceFilter(typeof(NotificationHelperActionFilter))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutGet()
        {
            try
            {
                User user = await _userService.FindUserByNameAsync(User.Identity.Name);
                return View(user);
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of user exception
            }
        }

        /// <summary>
        /// Handles the POST request for user logout.
        /// </summary>
        /// <returns>Redirects to the login page after successful logout.</returns>
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
                return RedirectToAction(nameof(Login)); // Redirect to login page after successful logout
            }
            catch (RequestException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of request exception
            }
        }

        /// <summary>
        /// Displays the user page.
        /// </summary>
        /// <returns>The user page view.</returns>
        [ServiceFilter(typeof(NotificationHelperActionFilter))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UserPage()
        {
            try
            {
                if (Request.Method != "GET")
                {
                    return NotFound(); // Return not found if request method is not GET
                }

                var user = await _userService.FindUserByNameAsync(User.Identity.Name);
                var posts = user.Posts.OrderBy(x => x.Date).ToList();
                await _notificationHelper.SetHasNotificationAsync();

                UserPageViewModel userPageVM = new UserPageViewModel(_context,
                    user,
                    await _userService.CountAdmiredAsync(user),
                    await _userService.CountAdmirersAsync(user),
                    await _postService.FindCommentsByUserAsync(user));

                userPageVM.User.Posts = posts;

                return View(userPageVM); // Return user page view with user information
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of user exception
            }
            catch (ArgumentException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of argument exception
            }
        }

        /// <summary>
        /// Displays the user page with parameters.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <returns>The user page view.</returns>
        [HttpGet]
        [ServiceFilter(typeof(NotificationHelperActionFilter))]
        [AllowAnonymous]
        public async Task<IActionResult> UserPageWithParams(string name)
        {
            try
            {
                if (Request.Method != "GET")
                {
                    return NotFound(); // Return not found if request method is not GET
                }

                User user = await _userService.FindUserByNameAsync(name);

                UserPageViewModel userPageVM = new UserPageViewModel(_context,
                    user,
                    await _userService.CountAdmiredAsync(user),
                    await _userService.CountAdmirersAsync(user),
                    await _postService.FindCommentsByUserAsync(user));

                return View("UserPage", userPageVM); // Return user page view with user information
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of user exception
            }
            catch (ArgumentException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page in case of argument exception
            }
        }

        /// <summary>
        /// Displays the view for adding a profile picture.
        /// </summary>
        /// <returns>The view for adding a profile picture.</returns>
        [HttpGet]
        [ServiceFilter(typeof(NotificationHelperActionFilter))]
        public async Task<IActionResult> AddPictureProfile()
        {
            if (Request.Method != "GET")
            {
                return NotFound(); // Return not found if request method is not GET
            }
            return View();
        }

        /// <summary>
        /// Handles the POST request for adding a profile picture.
        /// </summary>
        /// <param name="imageBytes">The byte representation of the image.</param>
        /// <returns>Redirects to the user page upon successful addition of the profile picture; otherwise, returns the add profile picture view with an error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddPictureProfile(string imageBytes)
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound(); // Return not found if request method is not POST
                }

                await _profilePictureService.AddPictureProfileAsync(imageBytes, await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction(nameof(UserPage));
            }
            catch (UserException)
            {
                return View(imageBytes); // Return add profile picture view with the image byte data in case of user exception
            }
        }

        /// <summary>
        /// Handles the removal of admiration for a user.
        /// </summary>
        /// <param name="id">The ID of the user for whom admiration is to be removed.</param>
        /// <returns>Redirects to the user page upon successful removal of admiration; otherwise, redirects to the user page with an error message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmiration(Guid id)
        {
            if (Request.Method != "POST")
            {
                // No action needed for non-POST requests
            }
            try
            {
                // Remove admiration for the specified user
                await _userService.RemoveAdmirationAsync(await _userService.FindUserByNameAsync(User.Identity.Name),
                    await _userService.FindUserByIdAsync(id));
                return RedirectToAction(nameof(UserPage)); // Redirect to user page after successful removal of admiration
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(UserPage), new { message = ex.Message }); // Redirect to user page with error message in case of user exception
            }
        }

        /// <summary>
        /// Handles the addition of admiration for a user.
        /// </summary>
        /// <param name="id">The ID of the user to be admired.</param>
        /// <returns>Redirects to the user page upon successful admiration; otherwise, redirects to the error page or home page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmiration(Guid id)
        {
            if (Request.Method != "POST")
            {
                return NotFound(); // Return not found if request method is not POST
            }

            try
            {
                User userAdmirer = await _userService.FindUserByNameAsync(User.Identity.Name);
                User userAdmired = await _userService.FindUserByIdAsync(id);

                // Add admiration for the specified user and add related notification
                await _userService.AddAdmirationAsync(userAdmirer, userAdmired);
                await _notificationService.AddAdmirationRelatedNotificationAsync(userAdmirer, userAdmired);

                return RedirectToAction(nameof(UserPage)); // Redirect to user page after successful admiration
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page with error message in case of user exception
            }
            catch (NotificationException ex)
            {
                return RedirectToAction("Index", "Home"); // Redirect to home page in case of notification exception
            }
        }

        /// <summary>
        /// Displays the admirers of a user.
        /// </summary>
        /// <param name="id">The ID of the user whose admirers are to be displayed.</param>
        /// <returns>The view displaying the admirers of the specified user.</returns>
        [HttpGet]
        [ServiceFilter(typeof(NotificationHelperActionFilter))]
        [AllowAnonymous]
        public async Task<IActionResult> ShowAdmirers(Guid id)
        {
            if (Request.Method != "GET")
            {
                return NotFound(); // Return not found if request method is not GET
            }
            try
            {
                ViewData["User"] = await _userService.FindUserByIdAsync(id);
                return View(await _userService.FindAdmirersOfUserAsync(await _userService.FindUserByIdAsync(id))); // Return view with admirers of the specified user
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page with error message in case of user exception
            }
        }

        /// <summary>
        /// Displays the users who admire the specified user.
        /// </summary>
        /// <param name="id">The ID of the user whose admirers are to be displayed.</param>
        /// <returns>The view displaying the users who admire the specified user.</returns>
        [HttpGet]
        [ServiceFilter(typeof(NotificationHelperActionFilter))]
        [AllowAnonymous]
        public async Task<IActionResult> ShowAdmired(Guid id)
        {
            if (Request.Method != "GET")
            {
                return NotFound(); // Return not found if request method is not GET
            }
            try
            {
                ViewData["User"] = await _userService.FindUserByIdAsync(id);
                return View(await _userService.FindAdmiredByUserAsnyc(await _userService.FindUserByIdAsync(id))); // Return view with users who admire the specified user
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message }); // Redirect to error page with error message in case of user exception
            }
        }

        /// <summary>
        /// Handles the request to change the description of the currently logged-in user.
        /// </summary>
        /// <param name="description">The new description to be set for the user.</param>
        /// <returns>Redirects to the user page upon successful description change; otherwise, redirects to the user page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeDescription(string description)
        {
            try
            {
                // Change the description for the currently logged-in user
                await _userService.ChangeDescriptionAsync(description, await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction(nameof(UserPage)); // Redirect to user page after successful description change
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(UserPage)); // Redirect to user page in case of user exception
            }
        }

        #region Controller Send Email To Recovery Password

        /// <summary>
        /// Displays the view for initiating the password recovery process.
        /// </summary>
        /// <returns>The view for initiating the password recovery process.</returns>
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request for initiating the password recovery process, allowing users to request a password reset email.
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
                    return RedirectToAction(nameof(ForgotPasswordConfirmation)); // Redirect to confirmation page upon successful email send
                }
                else
                {
                    ModelState.AddModelError("", "No is possible send the email!"); // Add error message if email sending fails
                }
            }

            return View(model); // Return to forgot password view with error message
        }

        /// <summary>
        /// Displays the view confirming that the password reset email has been sent.
        /// </summary>
        /// <returns>The view confirming that the password reset email has been sent.</returns>
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
