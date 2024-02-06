using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Services;
using System.Runtime.ExceptionServices;

namespace SocialWeave.Controllers
{
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService) 
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post) 
        {
            if (Request.Method != "POST") 
            {
                return NotFound();
            }

            if(ModelState.IsValid) 
            {
                await _postService.CreatePostAsync(post);    
                return View("Home", "Index");
            }

            return View(nameof(Create));
        }


    }
}
