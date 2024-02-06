using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.AbstractClasses;
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

        public async Task CreatePostAsync(Post post) 
        {
            if(post == null) 
            {
                throw new NullReferenceException("It is not possible to create a null post");
            }
            await _context.AddAsync(post);
            await _context.SaveChangesAsync();
        }

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

            foreach(var like in post.Like) 
            {
                _context.Likes.Remove(like);
            }

            foreach(var dislike in post.Dislikes) 
            {
                _context.Dislikes.Remove(dislike);
            }

            _context.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
