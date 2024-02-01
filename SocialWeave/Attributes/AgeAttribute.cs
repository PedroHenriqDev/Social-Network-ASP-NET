using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Attributes
{
    public class AgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;

            DateTime birthDate = (DateTime)value;
            TimeSpan duration = birthDate.Subtract(DateTime.Now);
            return duration.TotalDays > 4748;
        }
    }
}
