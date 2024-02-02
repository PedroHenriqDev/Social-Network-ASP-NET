using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Interfaces
{
    public interface IFeedBack
    {
        Guid Id { get; set; }
        User User { get; set; }   
    }   
}
