using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Attributes
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            string password = (string)value;
            bool hasNumber = password.Any(char.IsNumber);
            bool hasLetter = password.Any(char.IsLetter);

            return hasNumber && hasLetter;
        }
    }
}