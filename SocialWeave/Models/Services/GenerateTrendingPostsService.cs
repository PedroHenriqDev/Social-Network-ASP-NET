using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Service responsible for generating trending posts based on various factors.
    /// </summary>
    public class GenerateTrendingPostsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GenerateTrendingPostsService> _logger;

        /// <summary>
        /// Constructor for GenerateTrendingPostsService.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public GenerateTrendingPostsService(ApplicationDbContext context, ILogger<GenerateTrendingPostsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Generates trending posts based on the given posts, amount of posts, and current user.
        /// </summary>
        /// <param name="posts">The collection of posts to analyze.</param>
        /// <param name="amountOfPosts">The desired amount of trending posts to return.</param>
        /// <param name="currentUser">The current user.</param>
        /// <returns>A collection of trending posts.</returns>
        public async Task<IEnumerable<Post>> GenerateTrendingPostsAsync(IEnumerable<Post> posts, int amountOfPosts, User currentUser)
        {
            try
            {
                foreach (var post in posts)
                {
                    post.Likes = await _context.Likes
                                               .Where(l => l.Post.Id == post.Id)
                                               .ToListAsync();

                    post.Comments = await _context.Comments
                                                  .Include(c => c.User)
                                                  .Where(c => c.Post.Id == post.Id)
                                                  .OrderByDescending(c => c.Likes.Count())
                                                  .ToListAsync();
                }

                CalculateScores(posts, currentUser);

                var trendingPosts = posts.OrderByDescending(p => p.Score)
                                         .ThenByDescending(p => p.Likes.Count())
                                         .Where(p => p.Score >= -50)
                                         .Take(amountOfPosts)
                                         .ToList();

                _logger.LogInformation("Successful post generation");
                return trendingPosts;
            }
            catch(Exception ex) 
            {
                _logger.LogError("Error ocurred in generating posts");
                throw;
            }
        }

        //Override
        /// <summary>
        /// Generates trending posts based on the given posts and current user.
        /// </summary>
        /// <param name="posts">The collection of posts to analyze.</param>
        /// <param name="currentUser">The current user.</param>
        /// <returns>A collection of trending posts.</returns>
        public async Task<IEnumerable<Post>> GenerateTrendingPostsAsync(IEnumerable<Post> posts, User currentUser)
        {
            try
            {
                foreach (var post in posts)
                {
                    post.Likes = await _context.Likes
                                               .Where(l => l.Post.Id == post.Id)
                                               .ToListAsync();

                    post.Comments = await _context.Comments
                                                  .Include(c => c.User)
                                                  .Where(c => c.Post.Id == post.Id)
                                                  .ToListAsync();
                }

                CalculateScores(posts, currentUser);

                var trendingPosts = posts.OrderByDescending(p => p.Score)
                                         .ThenByDescending(p => p.Likes.Count())
                                         .Where(p => p.Score >= -50)
                                         .ToList();

                _logger.LogInformation("Successful post generation");
                return trendingPosts;
            }
            catch(Exception ex) 
            {
                _logger.LogError("Error ocurred in generating posts");
                throw;
            }
        }

        // Calculates scores for each post based on various factors
        /// <summary>
        /// Calculates the score for each post based on factors such as likes, comments, admirations, and post age.
        /// </summary>
        /// <param name="posts">The collection of posts for which scores are to be calculated.</param>
        /// <param name="currentUser">The current user.</param>
        private void CalculateScores(IEnumerable<Post> posts, User currentUser)
        {
            try
            {
                foreach (var post in posts)
                {
                    post.Score = 0;

                    // Increase score based on the number of likes
                    post.Score += post.Likes.Count;

                    // Increase score based on the number of unique commenters
                    post.Score += post.Comments.GroupBy(c => c.User.Id)
                                               .Count(group => group.Key != post.User.Id) + 1;

                    // Increase score based on the number of admirations received by the post author
                    post.Score += _context.Admirations.Count(a => a.UserAdmiredId == post.User.Id) * 0.1;

                    // Increase score if the current user admired the post author
                    post.Score += _context.Admirations.Count(a => a.UserAdmirerId == currentUser.Id && a.UserAdmiredId == post.User.Id) + 3;

                    // Decrease score based on the age of the post
                    var ageInDays = (DateTime.Now - post.Date).TotalDays;
                    if (ageInDays < 1)
                    {
                        post.Score += 2;
                    }
                    else if (ageInDays <= 365)
                    {
                        post.Score -= ageInDays / 3;
                    }
                    else
                    {
                        post.Score -= ageInDays + 350;
                    }
                }
                _logger.LogInformation("Success in score calculation");
            }
            catch (Exception ex) 
            {
                _logger.LogError("Error in score calculation");
            }
        }
    }
}
