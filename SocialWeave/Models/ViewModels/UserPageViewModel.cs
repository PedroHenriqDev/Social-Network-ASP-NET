using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.ViewModels
{
    public class UserPageViewModel
    {
        public User User { get; set; }
        public string Description { get; set; }

    }
}
