using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Like : IFeedBack
    {
        public Guid Id { get; set; }
        public User User { get; set; }

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
