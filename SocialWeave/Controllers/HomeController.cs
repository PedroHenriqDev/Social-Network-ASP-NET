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

namespace SocialWeave.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly AmountOfPostsHelper _amountOfPostsHelper;

        public HomeController(UserService userService, PostService postService, AmountOfPostsHelper amountOfPostsHelper)
        {
            _userService = userService;
            _postService = postService;
            _amountOfPostsHelper = amountOfPostsHelper;
        }

        /// <summary>
        /// Displays the home page with a list of posts authored by the logged-in user.
        /// </summary>
        /// <returns>The home page view.</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                int amountOfPosts = _amountOfPostsHelper.ReturnAmountOfPosts();

                IEnumerable<Post> posts = await _postService.FindPostsByGenerateTrendingAsync(await _userService.FindUserByNameAsync(User.Identity.Name), amountOfPosts);
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
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page with the error details.
        /// </summary>
        /// <returns>The error page view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
