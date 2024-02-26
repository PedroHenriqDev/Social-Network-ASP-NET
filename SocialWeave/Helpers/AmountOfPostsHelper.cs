using Microsoft.AspNetCore.Http;

namespace SocialWeave.Helpers
{
    /// <summary>
    /// Helper class for managing the amount of posts.
    /// </summary>
    public class AmountOfPostsHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Gets or sets the amount of posts.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmountOfPostsHelper"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public AmountOfPostsHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Returns the current amount of posts.
        /// </summary>
        /// <returns>The current amount of posts.</returns>
        public int ReturnAmountOfPosts()
        {
            ISession session = _httpContextAccessor.HttpContext.Session;

            if (session.Keys.Contains("AmountOfPosts"))
            {
                Amount = Convert.ToInt32(session.GetString("AmountOfPosts"));
                Amount += 10;
                return Amount;
            }
            else
            {
                Amount = 10;
                return Amount;
            }
        }

        /// <summary>
        /// Sets the amount of posts.
        /// </summary>
        /// <param name="amount">The amount of posts to set.</param>
        public void SetAmountOfPosts(int amount)
        {
            ISession session = _httpContextAccessor.HttpContext.Session;
            session.SetString("AmountOfPosts", amount.ToString());
        }
    }
}
