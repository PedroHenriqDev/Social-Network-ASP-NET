using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Interfaces
{
    public interface IComment
    {
        User User { get; set; }
        DateTime Date {  get; set; }
    }
}
