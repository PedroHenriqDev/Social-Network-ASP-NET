using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents an admiration relationship between two users.
    /// </summary>
    public class Admiration
    {
        /// <summary>
        /// The unique identifier for the admiration relationship.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the user who admires (initiates the admiration).
        /// </summary>
        [ForeignKey("UserAdmirer")]
        public Guid UserAdmirerId { get; set; }

        /// <summary>
        /// The user who admires (initiates the admiration).
        /// </summary>
        public User UserAdmirer { get; set; }

        /// <summary>
        /// The ID of the user who is admired.
        /// </summary>
        [ForeignKey("UserAdmired")]
        public Guid UserAdmiredId { get; set; }

        /// <summary>
        /// The user who is admired.
        /// </summary>
        public User UserAdmired { get; set; }

        /// <summary>
        /// Default constructor for the Admiration class.
        /// </summary>
        public Admiration()
        {
        }

        /// <summary>
        /// Parameterized constructor for the Admiration class.
        /// </summary>
        /// <param name="userConnected">The user who initiates the admiration.</param>
        /// <param name="userAdmired">The user who is admired.</param>
        /// <param name="id">The unique identifier for the admiration relationship.</param>
        public Admiration(User userConnected, User userAdmired, Guid id)
        {
            UserAdmirer = userConnected;
            UserAdmired = userAdmired;
            Id = id;
        }
    }
}
