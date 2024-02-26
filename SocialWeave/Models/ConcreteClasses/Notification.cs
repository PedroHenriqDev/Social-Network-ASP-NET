using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents a notification in the system.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The unique identifier for the notification.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the user to whom the notification is addressed.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The user to whom the notification is addressed.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The ID of the user involved in the notification.
        /// </summary>
        public Guid InvolvedUserId { get; set; }

        /// <summary>
        /// The user involved in the notification.
        /// </summary>
        public User InvolvedUser { get; set; }

        /// <summary>
        /// The content of the notification.
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        public string Content { get; set; }

        /// <summary>
        /// The date and time when the notification was created.
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Indicates whether the notification has been seen or not.
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        public bool WasSeen { get; set; }
    }
}
