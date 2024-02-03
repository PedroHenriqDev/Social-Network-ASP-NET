using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email {  get; set; }
    }
}
