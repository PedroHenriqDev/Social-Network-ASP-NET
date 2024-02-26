using SocialWeave.Models.AbstractClasses;
using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents a post without an image in the system.
    /// </summary>
    public sealed class PostWithoutImage : Post
    {
        /// <summary>
        /// Default constructor for the PostWithoutImage class.
        /// </summary>
        public PostWithoutImage()
        {
        }

        /// <summary>
        /// Parameterized constructor for the PostWithoutImage class.
        /// </summary>
        /// <param name="id">The unique identifier for the post.</param>
        /// <param name="description">The description of the post.</param>
        /// <param name="date">The date of creation of the post.</param>
        public PostWithoutImage(Guid id, [Required(ErrorMessage = "{0} is required")]
                                 [StringLength(50)] string description, DateTime date)
            : base(id, description, date)
        {
        }
    }
}
