using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SocialWeave.Attributes;

namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// ViewModel for creating a new user.
    /// </summary>
    public class UserCreateViewModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [HasLetter(ErrorMessage = "The field {0} is mandatory to have at least one letter")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The name must be 1 to 50 characters long!")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [PhoneNumber(ErrorMessage = "Invalid phone number format")]
        [DisplayName("Phone")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [StrongPassword(ErrorMessage = "Password must contain both letters and numbers")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [Age(ErrorMessage = "User must be at least 16 years old")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }
    }
}
