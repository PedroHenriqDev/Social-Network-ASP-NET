using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Interfaces
{
    public interface IFeedback
    {
        Guid Id { get; set; }
        User User { get; set; }
    }
}
