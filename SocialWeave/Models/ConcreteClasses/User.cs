using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SocialWeave.Attributes;
using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique identifier for the user.
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The salt value used for hashing the password.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// The profile picture of the user stored as a byte array.
        /// </summary>
        public byte[]? PictureProfile { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [HasLetter(ErrorMessage = "The field {0} is mandatory to have at least one letter")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The name must be 1 to 20 characters long!")]
        public string Name { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        public string Email { get; set; }

        /// <summary>
        /// The description or bio of the user.
        /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessage = "The {0} must be 0 to 2 characters long!")]
        public string? Description { get; set; }

        /// <summary>
        /// The birth date of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [Age(ErrorMessage = "It's mandatory be at least 16 years old!")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [StrongPassword(ErrorMessage = "Password must have, Numbers, Letter")]
        public string Password { get; set; }

        /// <summary>
        /// The reset token for the user's password reset.
        /// </summary>
        public string? ResetToken { get; set; }

        /// <summary>
        /// The date of creation of the user's account.
        /// </summary>
        public DateTime? DateCreation { get; set; } = DateTime.Now;

        /// <summary>
        /// The phone number of the user.
        /// </summary>
        [PhoneNumber(ErrorMessage = "Invalid phone number format")]
        [DisplayName("Phone")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// The list of posts made by the user.
        /// </summary>
        public List<Post>? Posts { get; set; }

        /// <summary>
        /// The list of users admired by the current user.
        /// </summary>
        public List<User>? Admirations { get; set; }

        /// <summary>
        /// Default constructor for the User class.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Parameterized constructor for the User class.
        /// </summary>
        /// <param name="id">The unique identifier for the user.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="phoneNumber">The phone number of the user.</param>
        /// <param name="birthDate">The birth date of the user.</param>
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
