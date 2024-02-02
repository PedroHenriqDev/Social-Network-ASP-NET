using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SocialWeave.Attributes
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            string phoneNumber = value.ToString();
            return Regex.IsMatch(phoneNumber, @"^\d{9}$|^\d{11}$|^\d{13}$");
        }

    }
}
