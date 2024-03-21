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
    /// <summary>
    /// Controller responsible for managing posts.
    /// </summary>
    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostController"/> class.
        /// </summary>
        /// <param name="postService">The service for handling post-related operations.</param>
        /// <param name="userService">The service for handling user-related operations.</param>
        /// <param name="notificationService">The service for handling notification-related operations.</param>
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

        /// <summary>
        /// Displays the view to edit a post.
        /// </summary>
        /// <param name="Id">The ID of the post to edit.</param>
        /// <returns>The view to edit the post.</returns>
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

        /// <summary>
        /// Edits a post with the provided description.
        /// </summary>
        /// <param name="description">The new description for the post.</param>
        /// <param name="postId">The ID of the post to edit.</param>
        /// <returns>Redirects to the user page.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string description, Guid postId)
        {
            try
            {
                await _postService.EditPostByDescriptionAsync(description, await _postService.FindPostByIdAsync(postId));
                return RedirectToAction("UserPage", "User");
            }
            catch (PostException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a post with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the post to delete.</param>
        /// <returns>Redirects to the user page.</returns>
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
            catch (PostException ex)
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
            catch (UserException)
            {
                return View();
            }
            catch (NotificationException)
            {
                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                TempData["CreateError"] = ex.ToString();
                return View(postVM);
            }
        }

        /// <summary>
        /// Creates a new post with an image based on the provided view model and image file.
        /// </summary>
        /// <param name="postImageVM">The view model containing the post details.</param>
        /// <param name="imageFile">The image file to be associated with the post.</param>
        /// <returns>Redirects to the home page.</returns>
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
                    await _postService.CreatePostAsync(postImageVM, userWhoPosted, imageFile);
                    await _notificationService.AddNotificationRelatedToPostAsync(userWhoPosted);
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                TempData["CreateError"] = ex.Message;
                return View(postImageVM);
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
            catch (UserException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
            catch (NotificationException)
            {
                return RedirectToAction("Index", "Home");
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

        /// <summary>
        /// Likes a post in the search page with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the post to like.</param>
        /// <returns>Redirects to the search page.</returns>
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
                return RedirectToAction("PageOfSearch", "Search");
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

        /// <summary>
        /// Dislikes a post in the search page with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the post to dislike.</param>
        /// <returns>Redirects to the search page.</returns>
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
        /// Displays the view to create a comment on a post.
        /// </summary>
        /// <param name="id">The ID of the post to comment on.</param>
        /// <returns>The view to create a comment.</returns>
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

        /// <summary>
        /// Creates a new comment on a post.
        /// </summary>
        /// <param name="commentVM">The view model containing the comment details.</param>
        /// <returns>Redirects to the home page.</returns>
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

        /// <summary>
        /// Deletes a comment with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the comment to delete.</param>
        /// <returns>Redirects to the user page.</returns>
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
            catch (PostException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Likes a comment with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the comment to like.</param>
        /// <returns>Redirects to the home page.</returns>
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

        /// <summary>
        /// Likes a comment in the search page with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the comment to like.</param>
        /// <returns>Redirects to the search page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LikeCommentInPageSearch(Guid id)
        {
            try
            {
                await _postService.AddLikeInCommentAsync(await _postService.FindCommentByIdAsync(id),
                    await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("PageOfSearch", "Search");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dislikes a comment in the search page with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the comment to dislike.</param>
        /// <returns>Redirects to the search page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DislikeCommentInPageSearch(Guid id)
        {
            try
            {
                await _postService.RemoveLikeInCommentAsync(await _postService.FindCommentByIdAsync(id),
                    await _userService.FindUserByNameAsync(User.Identity.Name));
                return RedirectToAction("PageOfSearch", "Search");
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dislikes a comment with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the comment to dislike.</param>
        /// <returns>Redirects to the home page.</returns>
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

        /// <summary>
        /// Adds admiration for a user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to admire.</param>
        /// <returns>Redirects to the home page.</returns>
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SavePost(Guid id) 
        {
            try 
            {
                User currentUser = await _userService.FindUserByNameAsync(User.Identity.Name);
                Post post = await _postService.FindPostByIdAsync(id);
                await _postService.SavePostAsync(post, currentUser);
                return RedirectToAction("Index", "Home");
            }
            catch(PostException ex) 
            {
                return RedirectToAction(nameof(Error), new { error = ex.Message });
            }
        }

        /// <summary>
        /// Displays the error page with the error details.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <returns>The error page view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



