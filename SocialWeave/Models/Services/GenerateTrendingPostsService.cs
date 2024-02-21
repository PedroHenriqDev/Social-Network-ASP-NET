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
    public class GenerateTrendingPostsService
    {
        private readonly ApplicationDbContext _context;

        public GenerateTrendingPostsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GenerateTrendingPostsAsync(IEnumerable<Post> posts, int amountOfPosts, User currentUser)
        {
            foreach (var post in posts)
            {
                post.Likes = await _context.Likes.Where(l => l.Post.Id == post.Id).ToListAsync();
                post.Comments = await _context.Comments.Include(c => c.User).Where(c => c.Post.Id == post.Id).OrderByDescending(c => c.Likes.Count()).ToListAsync();
            }

            CalculateScores(posts, currentUser);

            var trendingPosts = posts.OrderByDescending(p => p.Score)
                                     .ThenByDescending(p => p.Likes.Count())
                                     .Where(p => p.Score >= -50)
                                     .Take(amountOfPosts)
                                     .ToList();

            return trendingPosts;
        }

        public async Task<IEnumerable<Post>> GenerateTrendingPostsAsync(IEnumerable<Post> posts, User currentUser)
        {
            foreach (var post in posts)
            {
                post.Likes = await _context.Likes.Where(l => l.Post.Id == post.Id).ToListAsync();
                post.Comments = await _context.Comments.Include(c => c.User).Where(c => c.Post.Id == post.Id).ToListAsync();
            }

            CalculateScores(posts, currentUser);

            var trendingPosts = posts.OrderByDescending(p => p.Score)
                                     .ThenByDescending(p => p.Likes.Count())
                                     .Where(p => p.Score >= -50)
                                     .ToList();

            return trendingPosts;
        }

        private void CalculateScores(IEnumerable<Post> posts, User currentUser)
        {
            foreach (var post in posts)
            {
                post.Score = 0;

                post.Score += post.Likes.Count;

                post.Score += post.Comments.GroupBy(c => c.User.Id)
                                           .Count(group => group.Key != post.User.Id) + 1;

                post.Score += _context.Admirations.Count(a => a.UserAdmiredId == post.User.Id) * 0.1;

                post.Score += _context.Admirations.Count(a => a.UserAdmirerId == currentUser.Id && a.UserAdmiredId == post.User.Id) + 3;

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
        }
    }
}
