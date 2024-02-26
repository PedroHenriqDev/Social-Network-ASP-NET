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
using System.Linq;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerateTrendingPostsService _generateTrending;
        private readonly ILogger<PostService> _logger;

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
                    throw new NullReferenceException("It is not possible to create a null post");
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
                _logger.LogInformation("Post created with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while creating post.");
                throw;
            }
        }

        public async Task EditPostByDescriptionAsync(string newDescription, Post post)
        {
            try
            {
                if (newDescription == null || post == null || post.Description == newDescription)
                {
                    throw new PostException("An brutal error ocurred in edition!");
                }

                post.Score = 1;
                post.Description = newDescription;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Post created with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while editing post.");
                throw;
            }
        }

        public async Task CreatePostAsync(PostImageViewModel postImageVM, User user, IFormFile imageFile)
        {
            try
            {
                if (postImageVM == null || user == null || imageFile == null)
                {
                    throw new NullReferenceException("It is not possible to create a null post");
                }

                if(imageFile.Length > 20 * 1024 * 1024) 
                {
                    throw new ArgumentException("Image File size exceeds the limit of 20 MB");
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
                    _logger.LogInformation("Post created with successfully.");
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while creating post with image.");
                throw;
            }
        }

        private bool IsImageFile(IFormFile file)
        {
            if (file == null)
                return false;

            string[] permittedImageTypes = { "image/webp", "image/png", "image/jpeg", "image/jpg" };

            return permittedImageTypes.Contains(file.ContentType);
        }

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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error ocurred while find post by id.");
                throw;
            }
        }

        public async Task<IEnumerable<Comment>> FindCommentsByUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("Reference of user null!");
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
                _logger.LogError(ex, "Error ocurred while find comment by user.");
                throw;
            }
        }

        public async Task CompletePostAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new UserException("User null!");
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

                    _logger.LogInformation("Completed the post successfully.");
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while completed the post.");
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
                    throw new PostException("It is not possible to delete a null post");
                }

                _context.Comments.RemoveRange(post.Comments);
                _context.Likes.RemoveRange(post.Likes);
                _context.Remove(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted post successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while deleted the post.");
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

                _logger.LogInformation("Like added with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while added like in post.");
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
                _logger.LogInformation("Like removed with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while removed like in post.");
            }
        }

        public async Task AddAdmirationAsync(User user, User currentUser)
        {
            try
            {
                if (user == null || currentUser == null)
                {
                    throw new UserException("Impossible find this user!");
                }

                await _context.AddAsync(new Admiration(user, currentUser, new Guid()));
                await _context.SaveChangesAsync();
                _logger.LogInformation("Admiration added done with successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocurred while added admiration.");
            }
        }

        public async Task CreateCommentAsync(CommentViewModel commentVM, User user)
        {
            try
            {
                if (commentVM == null || user == null)
                {
                    throw new NullReferenceException();
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
                    throw new NullReferenceException();
                }

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Comment created with successfully.");
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while comment creation");
            }
        }

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
                _logger.LogInformation("Comment deleted with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while comment delete");
            }
        }

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
                _logger.LogInformation("Like added to the comment with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while added like in comment");
            }
        }

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
                    throw new NullReferenceException("Object null");
                }

                _context.Likes.Remove(likeToRemove);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Like removed to the comment with successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while remove like from the comment");
            }
        }

        /// <summary>
        /// Finds posts asynchronously excluding those authored by the specified user.
        /// </summary>
        /// <param name="user">The user whose posts should be excluded.</param>
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

                _logger.LogInformation("Successful post generation.");
                return await _generateTrending.GenerateTrendingPostsAsync(posts, quantityOfPost, user);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error ocurred while generated posts");
                throw;
            }
        }

        public async Task<Comment> FindCommentByIdAsync(Guid id)
        {
            try
            {
                if (id == null)
                {
                    throw new NullReferenceException("Object null!");
                }

                var comments =  await _context.Comments
                                .Include(x => x.User)
                                .Include(x => x.Post)
                                .Include(x => x.Post.User)
                                .Include(x => x.Likes)
                                .FirstOrDefaultAsync(x => x.Id == id);

                _logger.LogInformation("Successful find comment by id.");
                return comments;
            }
            catch (Exception ex) 
            {
                _logger.LogError("Error ocurred while find comment by id");
                throw;
            }
        }
    }
}
