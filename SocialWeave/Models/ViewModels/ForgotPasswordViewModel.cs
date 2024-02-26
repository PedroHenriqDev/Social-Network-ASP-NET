using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// View model for handling the forgot password process.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the email address associated with the user's account.
        /// </summary>
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
    }
}
