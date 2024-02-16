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

        public void RateByComment(IEnumerable<Post> posts) 
        {
            foreach (var post in posts) 
            {
                int userComment = 0;
                foreach (Comment comment in post.Comments)
                {
                    if (comment.Id == post.User.Id) 
                    {
                        userComment++;
                    }
                }
                post.Score += post.Comments.Count() - userComment;
            }
        }

        public void RateByAdmirer(IEnumerable<Post> posts) 
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
                TimeSpan duration = DateTime.Now.Subtract(post.Date);

                if(duration.TotalDays < 1) 
                {
                    post.Score += 2;
                }
                else if (duration.TotalDays >= 1)
                {
                    post.Score -= duration.TotalDays / 4;
                }
                else if (duration.TotalDays >= 30 && duration.TotalDays <= 365) 
                {
                    post.Score -= duration.TotalDays / 100 ;
                }
                else 
                {
                    post.Score -= duration.TotalDays + 350 ;
                }
            }
        }

        public IEnumerable<Post> GenerateByScore(IEnumerable<Post> posts)
        {
            RateByLike(posts);
            RateByComment(posts);
            RateByAdmirer(posts);
            RateByDate(posts);
            var sortedPosts = posts.OrderByDescending(x => x.Score).ThenByDescending(x => x.Likes.Count()).ToList();
            return sortedPosts;
        }
    }
}
