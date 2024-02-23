namespace SocialWeave.Helpers
{
    public class AmountOfPostsHelper
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public int Amount { get; set; }

        public AmountOfPostsHelper(IHttpContextAccessor httpContextAcessor) 
        {
            _httpContextAccessor = httpContextAcessor;
        }
        
        public int ReturnAmountOfPosts() 
        {
            ISession session = _httpContextAccessor.HttpContext.Session;

            if (session.Keys.Contains("AmountOfPosts"))
            {
                Amount =  Convert.ToInt32(session.GetString("AmountOfPosts"));

                Amount += 10;
                return Amount;
            }
            else
            {
                Amount = 10;
                return Amount;
            }
        }

        public void SetAmountOfPosts(int amount) 
        {
            ISession session = _httpContextAccessor.HttpContext.Session;
            session.SetString("AmountOfPosts", amount.ToString());
        }
    }
}
