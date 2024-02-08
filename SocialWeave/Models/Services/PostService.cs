using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System.Linq;
using System.Runtime.InteropServices;

namespace SocialWeave.Models.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task CreatePostAsync(PostViewModel postVM, User user) 
        {
            if(postVM == null) 
            {
                throw new NullReferenceException("It is not possible to create a null post");
            }

            PostWithoutImage post = new PostWithoutImage()
            {
                Date = DateTime.Now,
                User = user,
                Description = postVM.Description,
                Id = new Guid(),
            };

            await _context.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> FindPostById(Guid id) 
        {
            return await _context.Posts.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RemovePost(Post post)
        {
            if (post == null) 
            {
                throw new NullReferenceException("It is not possible to delete a null post");
            }

            foreach(var comment in post.Comments) 
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

        public async Task AddLikeInPostAsync(Post post, User user) 
        {

            if(post == null || user == null) 
            {
                throw new NullReferenceException();
            }

            Like like = new Like(new Guid(), user, post);
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLikeInPostAsync(Post post, User user)
        {
            if(user == null) 
            {
                throw new NullReferenceException();  
            }

            Like likeToRemove = await _context.Likes.FirstOrDefaultAsync(x => x.Post.Id == post.Id && x.User.Id == user.Id);

            if (likeToRemove == null)
            {
                throw new NullReferenceException();
            }

            _context.Likes.Remove(likeToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Post>> FindPostsAsync(User user) 
        {
            if(user == null) 
            {
                throw new UserException(" ");
            }

            IEnumerable<Post> posts =  await _context.Posts.Include(x => x.User)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .Where(x => x.User.Name != user.Name)
                .Take(20)
                .ToListAsync();

            if(posts.Count() == 0) 
            {
                throw new PostException(" ");
            }
            return posts;
        }
    }
}
