using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Attributes
{
    public class HasLetterAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;

            string name = (string)value;
            bool hasLetter = name.Any(char.IsLetter);
            return hasLetter;
        }
    }
}
