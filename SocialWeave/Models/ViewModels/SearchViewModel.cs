using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.ViewModels
{
    public class SearchViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Post> Posts { get; set; }

        public SearchViewModel(IEnumerable<User> users, IEnumerable<Post> posts)
        {
            Users = users;
            Posts = posts;
        }
    }
}
