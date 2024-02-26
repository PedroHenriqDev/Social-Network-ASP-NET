using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Attributes
{
    /// <summary>
    /// Validates that the provided date represents a user age of at least 13 years old.
    /// </summary>
    public class AgeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value represents a valid age.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value represents a valid age; otherwise, false.</returns>
        public override bool IsValid(object? value)
        {
            // If the value is null, it's considered invalid
            if (value == null)
                return false;

            // Cast the value to DateTime
            DateTime birthDate = (DateTime)value;

            // Calculate the duration between the birth date and the current date
            TimeSpan duration = DateTime.Now.Subtract(birthDate);

            // Check if the duration represents an age of at least 13 years old (4748 days)
            return duration.TotalDays > 4748;
        }
    }
}
