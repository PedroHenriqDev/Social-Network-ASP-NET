using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public PostService(ApplicationDbContext context, GenerateTrendingPostsService generateTrending)
        {
            _context = context;
            _generateTrending = generateTrending;
        }

        /// <summary>
        /// Creates a new post asynchronously.
        /// </summary>
        /// <param name="postVM">The view model containing the post details.</param>
        /// <param name="user">The user creating the post.</param>
        public async Task CreatePostAsync(PostViewModel postVM, User user)
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
        }

        public async Task EditPostByDescriptionAsync(string newDescription, Post post)
        {
            if(newDescription == null || post == null || post.Description == newDescription) 
            {
                throw new PostException("An brutal error ocurred in edition!");
            }

            post.Score = 1;
            post.Description = newDescription;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task CreatePostAsync(PostImageViewModel postImageVM, User user, IFormFile imageFile)
        {
            if (postImageVM == null || user == null || imageFile == null)
            {
                throw new NullReferenceException("It is not possible to create a null post");
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

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
            }
        }

        /// <summary>
        /// Finds a post by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the post to find.</param>
        /// <returns>The post with the specified ID.</returns>
        public async Task<Post> FindPostByIdAsync(Guid id)
        {
            return await _context.Posts.Include(x => x.User)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CompletePostAsync(User user) 
        {
            if(user == null) 
            {
                throw new UserException("User null!");
            }

            foreach (var post in user.Posts)
            {
                post.Likes = await _context.Likes.Include(x => x.User)
                      .Include(x => x.Post)
                      .Where(x => x.Post.Id == post.Id)
                      .ToListAsync();

                post.Comments = await _context.Comments.Include(x => x.User)
                    .Include(x => x.Likes)
                    .Where(x => x.Post.Id == post.Id)
                    .Take(20)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Removes a post and its related entities from the database asynchronously.
        /// </summary>
        /// <param name="post">The post to remove.</param>
        public async Task RemovePost(Post post)
        {
            if (post == null)
            {
                throw new NullReferenceException("It is not possible to delete a null post");
            }

            foreach (var comment in post.Comments)
            {
                _context.Comments.Remove(comment);
            }

            foreach (var like in post.Likes)
            {
                _context.Likes.Remove(like);
            }

            _context.Remove(post);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a like to a post asynchronously.
        /// </summary>
        /// <param name="post">The post to like.</param>
        /// <param name="user">The user who liked the post.</param>
        public async Task AddLikeInPostAsync(Post post, User user)
        {
            if (post == null || user == null)
            {
                throw new NullReferenceException("Post or user cannot be null");
            }

            Like like = new Like(Guid.NewGuid(), user, post);
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a like from a post asynchronously.
        /// </summary>
        /// <param name="post">The post to remove the like from.</param>
        /// <param name="user">The user who liked the post.</param>
        public async Task RemoveLikeInPostAsync(Post post, User user)
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
        }

        public async Task AddAdmirationAsync(User user, User currentUser) 
        {
            if(user == null || currentUser == null) 
            {
                throw new UserException("Impossible find this user!");
            }

            await _context.Admirations.AddAsync(new Admiration(user, currentUser, new Guid()));
            await _context.SaveChangesAsync();
        }

        public async Task CreateCommentAsync(CommentViewModel commentVM, User user) 
        {
            if(commentVM == null || user == null) 
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
        }

        public async Task AddLikeInCommentAsync(Comment comment, User user)
        {
            if(comment == null || user == null) 
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
        }

        public async Task RemoveLikeInCommentAsync(Comment comment, User user) 
        {
            if(comment == null || user == null) 
            {
                throw new NullReferenceException("Object null!");
            }

            Like likeToRemove = await _context.Likes.FirstOrDefaultAsync(x => x.Comment.Id == comment.Id && x.User.Id == user.Id);
            
            if(likeToRemove == null) 
            {
                throw new NullReferenceException("Object null");
            }

            _context.Likes.Remove(likeToRemove);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds posts asynchronously excluding those authored by the specified user.
        /// </summary>
        /// <param name="user">The user whose posts should be excluded.</param>
        /// <returns>A collection of posts.</returns>
        public async Task<IEnumerable<Post>> FindPostsByGenerateTrendingAsync(User user, int quantityOfPost)
        {
            if (user == null)
            {
                throw new UserException("User cannot be null");
            }

            IEnumerable<Post> posts = await _context.Posts.Include(x => x.User)
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
                post.Comments = await _context.Comments.Include(x => x.User)
                    .Include(x => x.Likes)
                    .Where(x => x.Post.Id == post.Id)
                    .ToListAsync();
            }

            return await _generateTrending.GenerateTrendingPostsAsync(posts, quantityOfPost, user);
        }

        public async Task<Comment> FindCommentByIdAsync(Guid id)
        {
            if(id == null) 
            {
                throw new NullReferenceException("Object null!");
            }

            return _context.Comments.FirstOrDefault(x => x.Id == id);
        }
    }
}
