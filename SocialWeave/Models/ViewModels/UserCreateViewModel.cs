using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SocialWeave.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "{0} is required!")]
        [HasLetter(ErrorMessage = "The field {0} is mandatory to have at least one letter")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The name must be 1 to 20 characters long!")]
        public string Name {  get; set; }

        [PhoneNumber(ErrorMessage = "Invalid phone number format")]
        [DisplayName("Phone")]
        public string PhoneNumber {  get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StrongPassword(ErrorMessage = "Password must have, Numbers, Letter")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Age(ErrorMessage = "It's mandatory be at least 16 years old!")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate {  get; set; }
    }
}
