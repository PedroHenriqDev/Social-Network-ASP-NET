using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.ViewModels
{
    public class UserPageViewModel
    {
        public User User { get; set; }
        public string CountAdmired { get; set; }
        public string CountAdmirer {get; set;}
    }
}
