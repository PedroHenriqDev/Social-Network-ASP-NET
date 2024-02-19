using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;

namespace SocialWeave.Models.Services
{
    public class SearchService
    {

        private readonly ApplicationDbContext _context;
        private readonly GenerateTrendingPostsService _generateTrending;

        public SearchService(ApplicationDbContext context, GenerateTrendingPostsService generateTrending)
        {
            _context = context;
            _generateTrending = generateTrending;
        }

        public async Task<IEnumerable<Post>> SearchPostByAdmiredAsync(User user)
        {
            var users = await _context.Admirations
                            .Include(x => x.UserAdmired)
                            .Include(x => x.UserAdmired.Posts)
                            .Where(x => x.UserAdmiredId != user.Id && x.UserAdmirerId == user.Id)
                            .Select(x => x.UserAdmired)
                            .ToListAsync();

            var posts = users.SelectMany(x => x.Posts)
                 .Where(x => x != null)
                 .ToList();

            foreach (var post in posts)
            {
                post.Comments = await _context.Comments
                    .Include(x => x.User)
                    .Include(x => x.Likes)
                    .Where(x => x.Post.Id == post.Id)
                    .ToListAsync();

                post.Likes = await _context.Likes
                    .Include(x => x.User)
                    .Include(x => x.Comment)
                    .Include(x => x.Post)
                    .Where(x => x.Post.Id == post.Id)
                    .ToListAsync();
            }


            return await _generateTrending.GenerateTrendingPostsAsync(posts, user);
        }

        public async Task<SearchViewModel> SearchUserByQueryAsync(string query, User currentUser) 
        {
            if(query == null) 
            {
                throw new SearchException();
            }

            var users = await _context.Users.Where(x => x.Name.ToLower().Contains(query.ToLower()))
                                            .ToListAsync();
               

            var posts = await _context.Posts.Where(x => x.Description.ToLower().Contains(query.ToLower()))
                                            .Include(x => x.User)
                                            .Include(x => x.Likes)
                                            .Include(x => x.Comments)
                                            .ToListAsync();

            foreach( var post in posts ) 
            {
                post.Comments = await _context.Comments
                    .Include(x => x.User)
                    .Include(x => x.Likes)
                    .Where(x => x.Post.Id == post.Id)
                    .ToListAsync();

                post.Likes = await _context.Likes
                    .Include(x => x.User)
                    .Include(x => x.Comment)
                    .Include(x => x.Post)
                    .Where(x => x.Post.Id == post.Id)
                    .ToListAsync();
            }

            
            SearchViewModel searchVM = new SearchViewModel(users, await _generateTrending.GenerateTrendingPostsAsync(posts, currentUser));
            return searchVM;
        }
    }
}
