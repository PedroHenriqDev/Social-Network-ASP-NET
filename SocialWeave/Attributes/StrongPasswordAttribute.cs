using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Attributes
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            string password = (string)value;
            bool hasCapital = password.Any(char.IsUpper);
            bool hasTiny = password.Any(char.IsLower);
            bool hasNumber = password.Any(char.IsNumber);
            bool hasLetter = password.Any(char.IsLetter);

            return hasCapital && hasTiny && hasNumber && hasLetter;

        }
    }
}
