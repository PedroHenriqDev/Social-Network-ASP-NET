using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// ViewModel for user authentication.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }
    }
}
