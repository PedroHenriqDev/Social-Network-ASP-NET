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

        public void RemovePost(Post post) 
        {
            foreach(var comment in post.Comments) 
            {
                _context.Comments.Remove(comment);
            }


        }

    }
}
