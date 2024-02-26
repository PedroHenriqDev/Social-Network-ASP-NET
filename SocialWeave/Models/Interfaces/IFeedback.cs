using SocialWeave.Models.ConcreteClasses;
using System;

namespace SocialWeave.Models.Interfaces
{
    /// <summary>
    /// Represents feedback given by a user on a post or a comment.
    /// </summary>
    public interface IFeedback
    {
        /// <summary>
        /// The unique identifier for the feedback.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// The user who gave the feedback.
        /// </summary>
        User User { get; set; }
    }
}
