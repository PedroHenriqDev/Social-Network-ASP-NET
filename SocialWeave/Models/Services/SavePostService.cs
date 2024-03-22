using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service class responsible for managing saved posts.
    /// </summary>
    public class SavePostService
    {
        private readonly ApplicationDbContext _context;
        private readonly PostService _postService;
        private readonly ILogger<SavePostService> _logger;

        /// <summary>
        /// Constructor for SavePostService.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="postService">The post service.</param>
        public SavePostService(
            ApplicationDbContext context,
            ILogger<SavePostService> logger,
            PostService postService)
        {
            _context = context;
            _logger = logger;
            _postService = postService;
        }

        /// <summary>
        /// Finds a saved post by its unique identifiers asynchronously.
        /// </summary>
        /// <param name="postId">The unique identifier of the post.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The saved post if found, otherwise null.</returns>
        public async Task<SavedPost> FindSavedPostByKeysAsync(Guid postId, Guid userId)
        {
            if (postId == Guid.Empty || userId == Guid.Empty)
            {
                throw new SavePostException("Error occurred while fetching the SavedPost object.");
            }
            return await _context.SavedPosts.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }

        /// <summary>
        /// Finds saved posts by user's unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A collection of saved posts.</returns>
        public async Task<IEnumerable<SavedPost>> FindSavedPostsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new SavePostException("Error occurred in finding saved posts.");
            }

            return await _context.SavedPosts.Where(x => x.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Saves a post asynchronously.
        /// </summary>
        /// <param name="post">The post to be saved.</param>
        /// <param name="currentUser">The current user.</param>
        public async Task SavePostAsync(Post post, User currentUser)
        {
            try
            {
                if (post == null || currentUser == null)
                {
                    throw new SavePostException("Error in saving post.");
                }

                SavedPost savedPost = new SavedPost
                {
                    User = currentUser,
                    UserId = currentUser.Id,
                    Post = post,
                    PostId = post.Id
                };

                await _context.SavedPosts.AddAsync(savedPost);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Post saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving the post.");
                throw;
            }
        }

        /// <summary>
        /// Completes saved posts asynchronously by fetching their corresponding post details.
        /// </summary>
        /// <param name="savedPosts">The collection of saved posts to be completed.</param>
        public async Task CompleteSavedPostsAsync(IEnumerable<SavedPost> savedPosts)
        {
            if (savedPosts.Any())
            {
                foreach (var savedPost in savedPosts)
                {
                    savedPost.Post = await _postService.FindPostByIdAsync(savedPost.PostId);
                }
            }
        }

        /// <summary>
        /// Removes a saved post asynchronously.
        /// </summary>
        /// <param name="post">The post to be removed from saved.</param>
        /// <param name="currentUser">The current user.</param>
        public async Task RemoveSavedPostAsync(Post post, User currentUser)
        {
            try
            {
                if (post == null || currentUser == null)
                {
                    throw new SavePostException("Error in removing saved post.");
                }

                SavedPost savedPost = await FindSavedPostByKeysAsync(post.Id, currentUser.Id);

                _context.SavedPosts.Remove(savedPost);
                await _context.SaveChangesAsync();

            }
            catch (SavePostException ex)
            {
                _logger.LogError(ex, "Error occurred while removing saved post.");
                throw;
            }
        }
    }
}
