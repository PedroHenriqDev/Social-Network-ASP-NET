using Microsoft.AspNetCore.Mvc;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using System.Diagnostics;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        
        public HomeController(UserService userService, PostService postService) 
        {
            _userService = userService;
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Post> posts = await _postService.FindPostsAsync(await _userService.FindUserByNameAsync(User.Identity.Name));
                return View(posts);
            }
            catch(PostException) 
            {
                return View();
            }
            catch (UserException) 
            {
                return View();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
