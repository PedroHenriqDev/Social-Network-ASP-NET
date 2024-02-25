using Microsoft.AspNetCore.Mvc;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using System.Diagnostics;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.AbstractClasses;
using System.Threading.Tasks;
using System.Collections.Generic;
using SocialWeave.Helpers;
using System.Globalization;

namespace SocialWeave.Controllers
{

    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly AmountOfPostsHelper _amountOfPostsHelper;
        private readonly SearchService _searchService;
        private readonly NotificationService _notificationService;
        private readonly NotificationHelper _notificationHelper;

        public HomeController(UserService userService, 
               PostService postService, 
               AmountOfPostsHelper amountOfPostsHelper, 
               SearchService searchService, NotificationService notificationService, 
               NotificationHelper notificationHelper)
        {
            _userService = userService;
            _postService = postService;
            _amountOfPostsHelper = amountOfPostsHelper;
            _searchService = searchService;
            _notificationService = notificationService;
            _notificationHelper = notificationHelper;
        }

        /// <summary>
        /// Displays the home page with a list of posts authored by the logged-in user.
        /// </summary>
        /// <returns>The home page view.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                int amountOfPosts = _amountOfPostsHelper.ReturnAmountOfPosts();
                User user = await _userService.FindUserByNameAsync(User.Identity.Name);
                IEnumerable<Post> posts = await _postService.FindPostsByGenerateTrendingAsync(user, amountOfPosts);
                ViewData["AmountOfPostsHelper"] = _amountOfPostsHelper;
                return View(posts);
            }
            catch (PostException)
            {
                return View();
            }
            catch (UserException)
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> IndexWithPostsAdmired() 
        {
            int amountPosts = _amountOfPostsHelper.ReturnAmountOfPosts();
            ViewData["PostAdmired"] = "Post from someone you admire";
            var user = await _userService.FindUserByNameAsync(User.Identity.Name);
            var posts = await _searchService.SearchPostByAdmiredAsync(user);
            return View("Index", posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAmountOfPosts(int amount) 
        {
            if(Request.Method != "POST") 
            {
                return NotFound();
            }
            _amountOfPostsHelper.SetAmountOfPosts(amount);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the About page.
        /// </summary>
        /// <returns>The About page view.</returns>
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page with the error details.
        /// </summary>
        /// <returns>The error page view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
