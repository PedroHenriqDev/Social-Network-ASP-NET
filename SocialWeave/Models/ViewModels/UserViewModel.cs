using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
