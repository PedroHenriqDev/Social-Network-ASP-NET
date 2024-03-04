using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents a comment made by a user on a post.
    /// </summary>
    public sealed class Comment : IFeedback
    {
        /// <summary>
        /// The unique identifier for the comment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The user who made the comment.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The text content of the comment.
        /// </summary>
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(100, MinimumLength = 1)]
        public string Text { get; set; }

        /// <summary>
        /// The collection of likes received by the comment.
        /// </summary>
        public IEnumerable<Like>? Likes { get; set; }

        /// <summary>
        /// The post to which the comment belongs.
        /// </summary>
        public Post? Post { get; set; }

        /// <summary>
        /// The date and time when the comment was created.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Default constructor for the Comment class.
        /// </summary>
        public Comment()
        {
        }

        /// <summary>
        /// Parameterized constructor for the Comment class.
        /// </summary>
        /// <param name="id">The unique identifier for the comment.</param>
        /// <param name="text">The text content of the comment.</param>
        /// <param name="user">The user who made the comment.</param>
        /// <param name="post">The post to which the comment belongs.</param>
        public Comment(Guid id, string text, User user, Post post)
        {
            Id = id;
            Text = text;
            User = user;
            Post = post;
        }
    }
}
