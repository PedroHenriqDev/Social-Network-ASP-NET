using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service responsible for operations related to posts.
    /// </summary>
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerateTrendingPostsService _generateTrending;
        private readonly ILogger<PostService> _logger;

        /// <summary>
        /// Constructor for the PostService class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="generateTrending">The service responsible for generating trending posts.</param>
        /// <param name="logger">The logger to log information, warnings, and errors.</param>
        public PostService(ApplicationDbContext context, GenerateTrendingPostsService generateTrending, ILogger<PostService> logger)
        {
            _context = context;
            _generateTrending = generateTrending;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new post asynchronously.
        /// </summary>
        /// <param name="postVM">The view model containing the post details.</param>
        /// <param name="user">The user creating the post.</param>
        public async Task CreatePostAsync(PostViewModel postVM, User user)
        {
            try
            {
                if (postVM == null || user == null)
                {
                    throw new NullReferenceException("Cannot create a null post");
                }

                PostWithoutImage post = new PostWithoutImage()
                {
                    Date = DateTime.Now,
                    User = user,
                    Description = postVM.Description,
                    Id = Guid.NewGuid(),
                    Score = 1
                };

                await _context.AddAsync(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Post created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating post.");
                throw;
            }
        }

        /// <summary>
        /// Edits a post by description asynchronously.
        /// </summary>
        /// <param name="newDescription">The new description for the post.</param>
        /// <param name="post">The post to be edited.</param>
        public async Task EditPostByDescriptionAsync(string newDescription, Post post)
        {
            try
            {
                if (newDescription == null || post == null || post.Description == newDescription)
                {
                    throw new PostException("A brutal error occurred in edition!");
                }

                post.Score = 1;
                post.Description = newDescription;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Post edited successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing post.");
                throw;
            }
        }

        /// <summary>
        /// Creates a new post with image asynchronously.
        /// </summary>
        /// <param name="postImageVM">The view model containing the post details with image.</param>
        /// <param name="user">The user creating the post.</param>
        /// <param name="imageFile">The image file to be associated with the post.</param>
        public async Task CreatePostAsync(PostImageViewModel postImageVM, User user, IFormFile imageFile)
        {
            try
            {
                if (postImageVM == null || user == null || imageFile == null)
                {
                    throw new NullReferenceException("Cannot create a null post");
                }

                if (imageFile.Length > 20 * 1024 * 1024)
                {
                    throw new ArgumentException("Image file size exceeds the limit of 20 MB");
                }

                if (!IsImageFile(imageFile))
                {
                    throw new ArgumentException("Invalid image file format.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    string fileName = SanitizeFileName(imageFile.Name);

                    PostWithImage post = new PostWithImage()
                    {
                        Date = DateTime.Now,
                        User = user,
                        Description = postImageVM.Description,
                        Id = Guid.NewGuid(),
                        Image = imageBytes,
                        Score = 1
                    };

                    await _context.Posts.AddAsync(post);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Post created successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating post with image.");
                throw;
            }
        }

        /// <summary>
        /// Checks if the file is an image.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>True if the file is an image, False otherwise.</returns>
        private bool IsImageFile(IFormFile file)
        {
            if (file == null)
                return false;

            string[] permittedImageTypes = { "image/webp", "image/png", "image/jpeg", "image/jpg" };

            return permittedImageTypes.Contains(file.ContentType);
        }

        /// <summary>
        /// Removes the file extension from the file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The file name without extension.</returns>
        private string SanitizeFileName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        /// <summary>
        /// Finds a post by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the post to find.</param>
        /// <returns>The post with the specified ID.</returns>
        public async Task<Post> FindPostByIdAsync(Guid id)
        {
            try
            {
                var resultPost = await _context.Posts.Include(x => x.User)
                                                     .Include(x => x.Likes)
                                                     .Include(x => x.Comments)
                                                     .FirstOrDefaultAsync(x => x.Id == id);

                resultPost.Likes = await _context.Likes
                                                 .Include(x => x.User)
                                                 .Include(x => x.Post)
                                                 .Where(x => x.Post.Id == resultPost.Id)
                                                 .ToListAsync();

                _logger.LogInformation("Search completed successfully.");
                return resultPost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding post by ID.");
                throw;
            }
        }

        /// <summary>
        /// Finds comments by user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom to find comments.</param>
        /// <returns>A collection of comments made by the user.</returns>
        public async Task<IEnumerable<Comment>> FindCommentsByUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("Null user reference!");
                }

                IEnumerable<Comment> comments = await _context.Comments
                                                              .Include(x => x.User)
                                                              .Include(x => x.Likes)
                                                              .Include(x => x.Post)
                                                              .Include(x => x.Post.User)
                                                              .Where(x => x.User.Id == user.Id)
                                                              .OrderByDescending(x => x.Likes.Count())
                                                              .ToListAsync();

                _logger.LogInformation("Search completed successfully.");
                return comments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding comments by user.");
                throw;
            }
        }

        /// <summary>
        /// Completes the details of a post asynchronously.
        /// </summary>
        /// <param name="user">The user for which to complete the posts.</param>
        public async Task CompletePostAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("Null user!");
                }

                foreach (var post in user.Posts)
                {
                    post.Likes = await _context.Likes
                                               .Include(x => x.User)
                                               .Include(x => x.Post)
                                               .Where(x => x.Post.Id == post.Id)
                                               .ToListAsync();

                    post.Comments = await _context.Comments
                                                  .Include(x => x.User)
                                                  .Include(x => x.Likes)
                                                  .Where(x => x.Post.Id == post.Id)
                                                  .Take(20)
                                                  .ToListAsync();

                    _logger.LogInformation("Post completed successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while completing the post.");
                throw;
            }
        }

        /// <summary>
        /// Removes a post and its related entities from the database asynchronously.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        public async Task DeletePostAsync(Post post)
        {
            try
            {

                if (post == null)
                {
                    throw new PostException("Cannot delete a null post");
                }

                _context.Comments.RemoveRange(post.Comments);
                _context.SavedPosts.RemoveRange(await _context.SavedPosts.Where(x => x.PostId == post.Id).ToListAsync());
                _context.Likes.RemoveRange(post.Likes);
                _context.Remove(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Post deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the post.");
                throw;
            }
        }

        /// <summary>
        /// Adds a like to a post asynchronously.
        /// </summary>
        /// <param name="post">The post to like.</param>
        /// <param name="user">The user who liked the post.</param>
        public async Task AddLikeInPostAsync(Post post, User user)
        {
            try
            {
                if (post == null || user == null)
                {
                    throw new NullReferenceException("Post or user cannot be null");
                }

                Like like = new Like(Guid.NewGuid(), user, post);
                await _context.Likes.AddAsync(like);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Like added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding like to post.");
                throw;
            }
        }

        /// <summary>
        /// Removes a like from a post asynchronously.
        /// </summary>
        /// <param name="post">The post to remove the like from.</param>
        /// <param name="user">The user who liked the post.</param>
        public async Task RemoveLikeInPostAsync(Post post, User user)
        {
            try
            {
                if (user == null || user == null)
                {
                    throw new NullReferenceException("User cannot be null");
                }

                Like likeToRemove = await _context.Likes.FirstOrDefaultAsync(x => x.Post.Id == post.Id && x.User.Id == user.Id);

                if (likeToRemove == null)
                {
                    throw new NullReferenceException("Object null!");
                }

                _context.Likes.Remove(likeToRemove);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Like removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing like from post.");
                throw;
            }
        }

        /// <summary>
        /// Adds admiration from one user to another asynchronously.
        /// </summary>
        /// <param name="user">The user who is being admired.</param>
        /// <param name="currentUser">The user who is admiring.</param>
        public async Task AddAdmirationAsync(User user, User currentUser)
        {
            try
            {
                if (user == null || currentUser == null)
                {
                    throw new UserException("Unable to find this user!");
                }

                await _context.AddAsync(new Admiration(user, currentUser, new Guid()));
                await _context.SaveChangesAsync();
                _logger.LogInformation("Admiration added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding admiration.");
                throw;
            }
        }

        /// <summary>
        /// Creates a new comment asynchronously.
        /// </summary>
        /// <param name="commentVM">The view model containing the comment details.</param>
        /// <param name="user">The user creating the comment.</param>
        public async Task CreateCommentAsync(CommentViewModel commentVM, User user)
        {
            try
            {
                if (commentVM == null || user == null)
                {
                    throw new NullReferenceException("Comment or user cannot be null.");
                }

                Post post = await FindPostByIdAsync(commentVM.PostId);
                Comment comment = new Comment()
                {
                    Id = new Guid(),
                    User = user,
                    Text = commentVM.Text,
                    Post = post

                };

                if (comment == null)
                {
                    throw new NullReferenceException("Comment cannot be null.");
                }

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Comment created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating comment.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a comment asynchronously.
        /// </summary>
        /// <param name="comment">The comment to delete.</param>
        public async Task DeleteCommentAsync(Comment comment)
        {
            try
            {
                if (comment == null)
                {
                    throw new PostException("Reference null!");
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Comment deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting comment.");
                throw;
            }
        }

        /// <summary>
        /// Adds a like to a comment asynchronously.
        /// </summary>
        /// <param name="comment">The comment to like.</param>
        /// <param name="user">The user who liked the comment.</param>
        public async Task AddLikeInCommentAsync(Comment comment, User user)
        {
            try
            {
                if (comment == null || user == null)
                {
                    throw new NullReferenceException("Object null!");
                }

                Like like = new Like()
                {
                    Id = new Guid(),
                    User = user,
                    Comment = comment
                };

                await _context.Likes.AddAsync(like);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Like added to the comment successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding like to comment.");
                throw;
            }
        }

        /// <summary>
        /// Removes a like from a comment asynchronously.
        /// </summary>
        /// <param name="comment">The comment to remove the like from.</param>
        /// <param name="user">The user who liked the comment.</param>
        public async Task RemoveLikeInCommentAsync(Comment comment, User user)
        {
            try
            {
                if (comment == null || user == null)
                {
                    throw new NullReferenceException("Object null!");
                }

                Like likeToRemove = await _context.Likes.FirstOrDefaultAsync(x => x.Comment.Id == comment.Id && x.User.Id == user.Id);

                if (likeToRemove == null)
                {
                    throw new NullReferenceException("Object null!");
                }

                _context.Likes.Remove(likeToRemove);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Like removed from the comment successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing like from the comment.");
                throw;
            }
        }

        /// <summary>
        /// Finds posts asynchronously excluding those authored by the specified user.
        /// </summary>
        /// <param name="user">The user whose posts should be excluded.</param>
        /// <param name="quantityOfPost">The quantity of posts to retrieve.</param>
        /// <returns>A collection of posts.</returns>
        public async Task<IEnumerable<Post>> FindPostsByGenerateTrendingAsync(User user, int quantityOfPost)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("User cannot be null");
                }

                IEnumerable<Post> posts = await _context.Posts
                                                        .Include(x => x.User)
                                                        .Include(x => x.Likes)
                                                        .Include(x => x.User.Admirations)
                                                        .Where(x => x.User.Id != user.Id)
                                                        .ToListAsync();

                if (posts.Count() == 0)
                {
                    throw new PostException("No posts found");
                }

                foreach (var post in posts)
                {
                    post.Comments = await _context.Comments
                                                  .Include(x => x.User)
                                                  .Include(x => x.Likes)
                                                  .Where(x => x.Post.Id == post.Id)
                                                  .ToListAsync();
                }

                foreach(var post in posts) 
                {
                    post.Likes = await _context.Likes
                                                .Include(x => x.User)
                                                .Where(x => x.Post.Id == post.Id)
                                                .ToListAsync();
                }

                _logger.LogInformation("Posts generated successfully.");
                return await _generateTrending.GenerateTrendingPostsAsync(posts, quantityOfPost, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating posts.");
                throw;
            }
        }

        /// <summary>
        /// Finds a comment by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the comment to find.</param>
        /// <returns>The comment with the specified ID.</returns>
        public async Task<Comment> FindCommentByIdAsync(Guid id)
        {
            try
            {
                if (id == null)
                {
                    throw new NullReferenceException("Object null!");
                }

                var comment = await _context.Comments
                                .Include(x => x.User)
                                .Include(x => x.Post)
                                .Include(x => x.Post.User)
                                .Include(x => x.Likes)
                                .FirstOrDefaultAsync(x => x.Id == id);

                _logger.LogInformation("Comment found successfully.");
                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding comment by ID.");
                throw;
            }
        }
    }
}
