using SocialWeave.Attributes;
using SocialWeave.Models.AbstractClasses;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The name must be 1 to 20 characters long!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [Age(ErrorMessage = "It's mandatory be at least 16 years old!")]
        public DateTime BirthDate {  get; set; }
        [Required(ErrorMessage = "{0} is required!")]
        [StrongPassword(ErrorMessage= "Password must have, Uppercase, Lowercase, Numbers, Letter")]
        public string Password { get; set; }
        public int PhoneNumber { get; set; }
        public List<Post> Posts { get; set; }

        public User() 
        {
        }

        public User(Guid id, string name, string email, string password, int phoneNumber, DateTime birthDate) 
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
