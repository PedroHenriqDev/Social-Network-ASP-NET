using Microsoft.Identity.Client;
using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class Like : Feedback
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Post? Post { get; set; }
        public Comment? Comment { get; set; }

        public Like() 
        {
        }

        public Like(Guid id, User user)
        {
            Id = id;
            User = user;
        }
    }
}
