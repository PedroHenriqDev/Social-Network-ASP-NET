namespace SocialWeave.Helpers
{
    public class AmountOfPostsHelper
    {

        private readonly IHttpContextAccessor _httpContextAcessor;

        public int Amount { get; set; }

        public AmountOfPostsHelper(IHttpContextAccessor httpContextAcessor) 
        {
            _httpContextAcessor = httpContextAcessor;
        }
        
        public int ReturnAmountOfPosts() 
        {
            ISession session = _httpContextAcessor.HttpContext.Session;

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
            ISession session = _httpContextAcessor.HttpContext.Session;
            session.SetString("AmountOfPosts", amount.ToString());
        }

    }
}
