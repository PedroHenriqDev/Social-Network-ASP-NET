using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SocialWeave.Attributes
{
    /// <summary>
    /// Validates that the provided value represents a valid phone number format.
    /// </summary>
    public class PhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value represents a valid phone number format.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value represents a valid phone number format; otherwise, false.</returns>
        public override bool IsValid(object value)
        {
            // If the value is null, it's considered valid (since it's optional)
            if (value == null)
                return true;

            // Convert the value to string
            string phoneNumber = value.ToString();

            // Define a regular expression pattern to match valid phone number formats
            // The pattern matches 9, 11, or 13 consecutive digits
            string pattern = @"^\d{9}$|^\d{11}$|^\d{13}$";

            // Check if the phone number matches the pattern using regex
            bool isValidPhoneNumber = Regex.IsMatch(phoneNumber, pattern);

            return isValidPhoneNumber;
        }
    }
}
