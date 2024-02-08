using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using System.Runtime.ExceptionServices;

namespace SocialWeave.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly UserService _userService;

        public PostController(PostService postService, UserService userService) 
        {
            _postService = postService;
            _userService = userService;
        }

        public IActionResult ChoosePostType()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreatePostImage() 
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(Guid id) 
        {
            try
            {
                await _postService.AddLikeAsync(await _postService.FindPostById(id), 
                    await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("Index", "Home");
            }
            catch (NullReferenceException) 
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostViewModel postVM) 
        {
            if (Request.Method != "POST") 
            {
                return NotFound();
            }

            if(ModelState.IsValid) 
            {
                await _postService.CreatePostAsync(postVM, await _userService.FindUserByNameAsync(User.Identity.Name));    
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(CreatePost));
        }
    }
}
