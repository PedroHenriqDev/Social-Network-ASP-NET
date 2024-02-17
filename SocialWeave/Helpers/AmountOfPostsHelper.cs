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

                for (int i = Amount; i < 1000; i++) 
                {
                    if (i % 10 == 0)
                    {
                        Amount += i;
                    }
                    else 
                    {
                        Amount += 1;
                    }
                }
                return Amount;
            }
            else
            {
                return 10;
            }
        }

        public void SetAmountOfPosts(int amount) 
        {
            ISession session = _httpContextAcessor.HttpContext.Session;
            session.SetString("AmountOfPosts", amount.ToString());
        }

    }
}
