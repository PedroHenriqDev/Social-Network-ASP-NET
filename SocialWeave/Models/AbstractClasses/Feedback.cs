using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.AbstractClasses
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public User User { get; set; }
    }
}
