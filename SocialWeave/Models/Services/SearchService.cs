using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;

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

            return await _generateTrending.GenerateTrendingPostsAsync(posts, user);
        }

    }
}
