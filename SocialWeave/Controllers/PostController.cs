using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using System.Diagnostics;

namespace SocialWeave.Controllers
{

    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public PostController(PostService postService, UserService userService, NotificationService notificationService)
        {
            _postService = postService;
            _userService = userService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Displays the view to choose the type of post to create.
        /// </summary>
        /// <returns>The view to choose the post type.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChoosePostType()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            try
            {
                if (Request.Method != "POST")
                {
                    return RedirectToAction("UserPage", "User");
                }
                await _postService.DeletePostAsync(await _postService.FindPostByIdAsync(id));
                return RedirectToAction("UserPage", "User");
            }
            catch(PostException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Displays the view to create a post with an image.
        /// </summary>
        /// <returns>The view to create a post with an image.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreatePostImage()
        {
            return View();
        }

        /// <summary>
        /// Displays the view to create a text post.
        /// </summary>
        /// <returns>The view to create a text post.</returns>
        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
                    User userWhoPosted = await _userService.FindUserByNameAsync(User.Identity.Name);
                    await _postService.CreatePostAsync(postVM, userWhoPosted);
                    await _notificationService.AddNotificationRelatedToPostAsync(userWhoPosted);
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch(UserException) 
            {
               return View();
            }
            catch (NotificationException) 
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePostImage(PostImageViewModel postImageVM, IFormFile imageFile)
        {
            try
            {
                ModelState.Remove(nameof(postImageVM.Image));
                if (ModelState.IsValid)
                {
                    User userWhoPosted = await _userService.FindUserByNameAsync(User.Identity.Name);
                    await _postService.CreatePostAsync(postImageVM, userWhoPosted, imageFile );
                    await _notificationService.AddNotificationRelatedToPostAsync(userWhoPosted);
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
        [AllowAnonymous]
        public async Task<IActionResult> Like(Guid id)
        {
            try
            {
                Post post = await _postService.FindPostByIdAsync(id);
                User userLike = await _userService.FindUserByNameAsync(User.Identity.Name);
                User userPost = await _userService.FindUserByNameAsync(post.User.Name);

                await _postService.AddLikeInPostAsync(post, userLike);
                await _notificationService.AddNotificationRelatedLikeAsync(userLike, userPost);
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
            catch (NotificationException) 
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LikeInPageSearch(Guid id)
        {
            try
            {
                Post post = await _postService.FindPostByIdAsync(id);
                User userLike = await _userService.FindUserByNameAsync(User.Identity.Name);
                User userPost = await _userService.FindUserByNameAsync(post.User.Name);

                await _postService.AddLikeInPostAsync(post, userLike);
                await _notificationService.AddNotificationRelatedLikeAsync(userLike, userPost);
                return RedirectToAction("UserPage", "User");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (NotificationException)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DislikeInPageSearch(Guid id)
        {
            try
            {
                await _postService.RemoveLikeInPostAsync(await _postService.FindPostByIdAsync(id),
                await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("PageOfSearch", "Search");
            }
            catch (NullReferenceException ex)
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> CreateComment(CommentViewModel commentVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User userComment = await _userService.FindUserByNameAsync(User.Identity.Name);
                    Post post = await _postService.FindPostByIdAsync(commentVM.PostId);
                    User userPost = await _userService.FindUserByNameAsync(post.User.Name);

                    await _postService.CreateCommentAsync(commentVM, userComment);
                    await _notificationService.AddNotificationRelatedCommentAsync(userComment, userPost);

                    return RedirectToAction("Index", "Home");
                }

                return View(commentVM);
            }
            catch (NullReferenceException)
            {
                return View(commentVM);
            }
            catch (NotificationException)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteComment(Guid id) 
        {
            try 
            {
                await _postService.DeleteCommentAsync(await _postService.FindCommentByIdAsync(id));
                return RedirectToAction("UserPage", "User");
            }
            catch(PostException ex) 
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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



