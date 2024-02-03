using Microsoft.SqlServer.Server;
using SocialWeave.Attributes;
using SocialWeave.Models.AbstractClasses;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace SocialWeave.Models.ConcreteClasses
{
    public class User : IdentityUser
    {
        public string Salt {  get; set; }

        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The name must be 1 to 20 characters long!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Age(ErrorMessage = "It's mandatory be at least 16 years old!")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate {  get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StrongPassword(ErrorMessage= "Password must have, Numbers, Letter")]
        public string Password { get; set; }

        [PhoneNumber(ErrorMessage = "Invalid phone number format")]
        [DisplayName("Phone")]
        public string? PhoneNumber { get; set; }
        public List<Post>? Posts { get; set; }

        public User() 
        {
        }

        public User(Guid id, string name, string email, string password, string phoneNumber, DateTime birthDate) 
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
        }
    }
}
