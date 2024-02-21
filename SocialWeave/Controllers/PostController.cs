using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using System.Diagnostics;

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

        /// <summary>
        /// Displays the view to choose the type of post to create.
        /// </summary>
        /// <returns>The view to choose the post type.</returns>
        public IActionResult ChoosePostType()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id) 
        {
            Post post = await _postService.FindPostByIdAsync(Id);
            if (post != null)
            {
                return View(post);
            }
            return RedirectToAction(nameof(Error), new { message = "Post null!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string description, Guid postId) 
        {
            try 
            {
                await _postService.EditPostByDescriptionAsync(description, await _postService.FindPostByIdAsync(postId));
                return RedirectToAction("UserPage", "User");
            }
            catch(PostException ex)
            {
                return RedirectToAction(nameof(Error), new {message = ex.Message});
            }
        }

        /// <summary>
        /// Displays the view to create a post with an image.
        /// </summary>
        /// <returns>The view to create a post with an image.</returns>
        [HttpGet]
        public IActionResult CreatePostImage()
        {
            return View();
        }

        /// <summary>
        /// Displays the view to create a text post.
        /// </summary>
        /// <returns>The view to create a text post.</returns>
        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }

        /// <summary>
        /// Creates a new post based on the provided view model.
        /// </summary>
        /// <param name="postVM">The view model containing the post details.</param>
        /// <returns>Redirects to the home page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostViewModel postVM)
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    await _postService.CreatePostAsync(postVM, await _userService.FindUserByNameAsync(User.Identity.Name));
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction(nameof(CreatePost));
            }
            catch(UserException) 
            {
               return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePostImage(PostImageViewModel postImageVM, IFormFile imageFile)
        {
            try
            {
                ModelState.Remove(nameof(postImageVM.Image));
                if (ModelState.IsValid)
                {

                    await _postService.CreatePostAsync(postImageVM, await _userService.FindUserByNameAsync(User.Identity.Name), imageFile );
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Likes a post with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the post to like.</param>
        /// <returns>Redirects to the home page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(Guid id)
        {
            try
            {
                await _postService.AddLikeInPostAsync(await _postService.FindPostByIdAsync(id),
                await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("Index", "Home");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch(UserException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dislikes a post with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the post to dislike.</param>
        /// <returns>Redirects to the home page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dislike(Guid id)
        {
            try
            {
                await _postService.RemoveLikeInPostAsync(await _postService.FindPostByIdAsync(id),
                await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("Index", "Home");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateComment(Guid id)
        {
            try
            {
                return View(new CommentViewModel { PostId = id });
            }
            catch (PostException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(CommentViewModel commentVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _postService.CreateCommentAsync(commentVM, await _userService.FindUserByNameAsync(User.Identity.Name));
                    return RedirectToAction("Index", "Home");
                }

                return View(commentVM);
            }
            catch (NullReferenceException)
            {
                return View(commentVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeComment(Guid id)
        {
            try
            {
                await _postService.AddLikeInCommentAsync(await _postService.FindCommentByIdAsync(id),
                    await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("Index", "Home");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DislikeComment(Guid id)
        {
            try
            {
                await _postService.RemoveLikeInCommentAsync(await _postService.FindCommentByIdAsync(id),
                    await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("Index", "Home");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmiration(Guid id)
        {
            try
            {
                await _postService.AddAdmirationAsync(await _userService.FindUserByIdAsync(id), await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("Index", "Home");
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
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



