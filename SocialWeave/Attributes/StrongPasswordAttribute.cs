using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SocialWeave.Attributes
{
    /// <summary>
    /// Validates that the provided value represents a strong password containing at least one letter and one number.
    /// </summary>
    public class StrongPasswordAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value represents a strong password.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value represents a strong password; otherwise, false.</returns>
        public override bool IsValid(object? value)
        {
            // If the value is null, it's considered invalid
            if (value == null)
                return false;

            // Convert the value to string
            string password = value.ToString();

            // Check if the password contains at least one number and one letter
            bool hasNumber = password.Any(char.IsNumber);
            bool hasLetter = password.Any(char.IsLetter);

            // Return true if the password contains both a number and a letter
            return hasNumber && hasLetter;
        }
    }
}
