using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SocialWeave.Attributes
{
    /// <summary>
    /// Validates that the provided value contains at least one letter.
    /// </summary>
    public class HasLetterAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value contains at least one letter.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value contains at least one letter; otherwise, false.</returns>
        public override bool IsValid(object? value)
        {
            // If the value is null, it's considered invalid
            if (value == null)
                return false;

            // Cast the value to string
            string stringValue = value.ToString();

            // Check if the string contains at least one letter
            bool hasLetter = stringValue.Any(char.IsLetter);

            return hasLetter;
        }
    }
}
