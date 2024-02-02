using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Dislike : IFeedBack
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        public Dislike()
        {
        }

        public Dislike(Guid id, User user)
        {
            Id = id;
            User = user;
        }
    }
}
