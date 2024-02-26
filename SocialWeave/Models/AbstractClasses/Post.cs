using SocialWeave.Models.ConcreteClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.AbstractClasses
{
    /// <summary>
    /// Abstract class representing a post.
    /// </summary>
    public abstract class Post
    {
        /// <summary>
        /// Gets or sets the ID of the post.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the description of the post.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date when the post was created.
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the list of likes associated with the post.
        /// </summary>
        public List<Like>? Likes { get; set; }

        /// <summary>
        /// Gets or sets the list of comments associated with the post.
        /// </summary>
        public IEnumerable<Comment>? Comments { get; set; }

        /// <summary>
        /// Gets or sets the user who created the post.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the score of the post.
        /// </summary>
        public double? Score { set; get; }

        /// <summary>
        /// Default constructor for the Post class.
        /// </summary>
        public Post()
        {
        }

        /// <summary>
        /// Parameterized constructor for the Post class.
        /// </summary>
        /// <param name="id">The ID of the post.</param>
        /// <param name="description">The description of the post.</param>
        /// <param name="date">The date when the post was created.</param>
        public Post(Guid id, string description, DateTime date)
        {
            Id = id;
            Description = description;
            Date = date;
        }
    }
}
