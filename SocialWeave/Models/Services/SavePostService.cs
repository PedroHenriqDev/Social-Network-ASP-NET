using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Services
{
    public class SavePostService
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<SavePostService> _logger;

        public SavePostService(ApplicationDbContext context, ILogger<SavePostService> logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SavedPost> FindSavedPostByKeysAsync(Guid postId, Guid userId)
        {
            if(postId == null || userId == null) 
            {
                throw new SavePostException("Error occurred while fetching the SavedPost object is taking place, and ");
            }
            return await _context.SavedPosts.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }

        public async Task SavePostAsync(Post post, User currentUser)
        {
            try
            {
                if (post == null || currentUser == null)
                {
                    throw new SavePostException("Error in save post!");
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
                _logger.LogError(ex, "Error ocurred while saving from the post");
                throw;
            }
        }

        public async Task RemoveSavedPostAsync(Post post, User currentUser)
        {
            try
            {
                if (post == null || currentUser == null)
                {
                    throw new SavePostException("Error in remove saved post");
                }

                SavedPost savedPost = await FindSavedPostByKeysAsync(post.Id, currentUser.Id);

                _context.SavedPosts.Remove(savedPost);
                await _context.SaveChangesAsync();

            }
            catch (SavePostException ex)
            {
                _logger.LogError(ex, "Error ocurred while remove saved post");
                throw;
            }
        }
    }
}
