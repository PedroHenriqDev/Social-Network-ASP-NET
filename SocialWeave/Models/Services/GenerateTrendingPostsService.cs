using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using System.Runtime.CompilerServices;

namespace SocialWeave.Models.Services
{
    public class GenerateTrendingPostsService
    {
        private readonly ApplicationDbContext _context;

        public GenerateTrendingPostsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void RateByLike(IEnumerable<Post> posts) 
        {
            foreach (var post in posts) 
            {
                post.Score = post.Likes.Count();
            }
        }


        public void RateByAdmirersAsync(IEnumerable<Post> posts) 
        {
            foreach (var post in posts) 
            {
                foreach (var admiration in _context.Admirations)
                {
                    if(admiration.UserAdmiredId == post.User.Id)
                    {
                        post.Score += 1;
                    }
                }
            }
        }

        public void RateByDate(IEnumerable<Post> posts) 
        {
            foreach (var post in posts)
            {
                if (post.Date.Day > 1 && post.Date.Day < 31)
                {
                    post.Score -= post.Date.Day / 5;
                }
                else if (post.Date.Month < 1 && post.Date.Month < 13) 
                {
                    post.Score -= post.Date.Month + 5;
                }
                else 
                {
                    post.Score -= post.Date.Year + 20;
                }
            }
        }

        public void GenerateByScore(IEnumerable<Post> posts)
        {
            RateByLike(posts);
            RateByAdmirersAsync(posts);
            RateByDate(posts);
            posts.OrderByDescending(x => x.Score).ThenByDescending(x => x.Likes);
        }

    }
}
