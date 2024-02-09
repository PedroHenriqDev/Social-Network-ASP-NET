using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new post asynchronously.
        /// </summary>
        /// <param name="postVM">The view model containing the post details.</param>
        /// <param name="user">The user creating the post.</param>
        public async Task CreatePostAsync(PostViewModel postVM, User user)
        {
            if (postVM == null)
            {
                throw new NullReferenceException("It is not possible to create a null post");
            }

            PostWithoutImage post = new PostWithoutImage()
            {
                Date = DateTime.Now,
                User = user,
                Description = postVM.Description,
                Id = Guid.NewGuid(),
            };

            await _context.AddAsync(post);
            await _context.SaveChangesAsync();
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
            if (user == null)
            {
                throw new NullReferenceException("User cannot be null");
            }

            Like likeToRemove = await _context.Likes.FirstOrDefaultAsync(x => x.Post.Id == post.Id && x.User.Id == user.Id);

            if (likeToRemove == null)
            {
                throw new NullReferenceException("Like not found");
            }

            _context.Likes.Remove(likeToRemove);
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

        /// <summary>
        /// Finds posts asynchronously excluding those authored by the specified user.
        /// </summary>
        /// <param name="user">The user whose posts should be excluded.</param>
        /// <returns>A collection of posts.</returns>
        public async Task<IEnumerable<Post>> FindPostsAsync(User user)
        {
            if (user == null)
            {
                throw new UserException("User cannot be null");
            }

            IEnumerable<Post> posts = await _context.Posts.Include(x => x.User)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .Where(x => x.User.Name != user.Name)
                .Take(20)
                .ToListAsync();

            if (posts.Count() == 0)
            {
                throw new PostException("No posts found");
            }
            return posts;
        }
    }
}
