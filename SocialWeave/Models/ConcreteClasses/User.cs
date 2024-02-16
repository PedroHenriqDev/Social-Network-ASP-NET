using Microsoft.SqlServer.Server;
using SocialWeave.Attributes;
using SocialWeave.Models.AbstractClasses;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    public class User
    {
        public string Salt { get; set; }

        [Required]
        [Key]
        public Guid Id { get; set; }
        public byte[]? PictureProfile { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [HasLetter(ErrorMessage = "The field {0} is mandatory to have at least one letter")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The name must be 1 to 20 characters long!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        public string Email { get; set; }

        [StringLength(200, MinimumLength = 0, ErrorMessage = "The {0} must be 0 to 2 characters long!")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Age(ErrorMessage = "It's mandatory be at least 16 years old!")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StrongPassword(ErrorMessage = "Password must have, Numbers, Letter")]
        public string Password { get; set; }

        public string? ResetToken { get; set; }
        public DateTime? DateCreation { get; set; } = DateTime.Now;

        [PhoneNumber(ErrorMessage = "Invalid phone number format")]
        [DisplayName("Phone")]
        public string? PhoneNumber { get; set; }
        public List<Post>? Posts { get; set; }
        public List<User>? Admirations { get; set; }

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
