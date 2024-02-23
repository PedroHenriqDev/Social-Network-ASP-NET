using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid InvolvedUserId { get; set; }
        public User InvolvedUser { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool WasSeen { get; set; }
    }
}
