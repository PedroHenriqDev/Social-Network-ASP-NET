using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using SocialWeave.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service responsible for performing search operations in the system.
    /// </summary>
    public class SearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerateTrendingPostsService _generateTrending;
        private readonly UserService _userService;
        private readonly ILogger<SearchService> _logger;

        /// <summary>
        /// Constructor for the search service.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="generateTrending">The trending posts generation service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="logger">The logger to log information, warnings, and errors.</param>
        public SearchService(ApplicationDbContext context, GenerateTrendingPostsService generateTrending, UserService userService, ILogger<SearchService> logger)
        {
            _context = context;
            _generateTrending = generateTrending;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Searches for posts made by users admired by the specified user.
        /// </summary>
        /// <param name="user">The user for whom posts from admired users will be searched.</param>
        /// <returns>A collection of posts made by admired users.</returns>
        public async Task<IEnumerable<Post>> SearchPostByAdmiredAsync(User user)
        {
            try
            {
                var users = await _userService.ReturnAdmiredFromUserAsync(user);

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

                _logger.LogInformation("Search for posts by the user's admirers done successfully");
                return await _generateTrending.GenerateTrendingPostsAsync(posts, user);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Errors in searching for posts by the user's admirers");
                throw;
            }
        }

        /// <summary>
        /// Searches for users and posts that match the specified query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="currentUser">The current user performing the search.</param>
        /// <returns>A search view model containing users and posts matching the query.</returns>
        public async Task<SearchViewModel> SearchUsersAndPostsByQueryAsync(string query, User currentUser)
        {
            try
            {
                if (query == null)
                {
                    throw new SearchException("The search query cannot be null.");
                }

                var users = await _context.Users.Where(x => x.Name.ToLower().Contains(query.ToLower()))
                                                .ToListAsync();

                var posts = await _context.Posts.Where(x => x.Description.ToLower().Contains(query.ToLower()))
                                                .Include(x => x.User)
                                                .Include(x => x.Likes)
                                                .Include(x => x.Comments)
                                                .ToListAsync();

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

                _logger.LogInformation("Search for content following a successful query.");
                return new SearchViewModel(users, await _generateTrending.GenerateTrendingPostsAsync(posts, currentUser));
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error when searching for a query");
                throw;
            }
        }
    }
}
