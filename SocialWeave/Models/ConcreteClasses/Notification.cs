using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Date {  get; set; }

        [Required]
        public bool WasSeen { get; set; }
    }
}
