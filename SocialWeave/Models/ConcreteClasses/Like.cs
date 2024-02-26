using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Interfaces;
using System;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents a like given by a user to a post or a comment.
    /// </summary>
    public sealed class Like : IFeedback
    {
        /// <summary>
        /// The unique identifier for the like.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The user who gave the like.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The comment that is liked. Can be null if the like is for a post.
        /// </summary>
        public Comment? Comment { get; set; }

        /// <summary>
        /// The post that is liked. Can be null if the like is for a comment.
        /// </summary>
        public Post? Post { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Like()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Like"/> class for a post.
        /// </summary>
        /// <param name="id">The unique identifier for the like.</param>
        /// <param name="user">The user who gave the like.</param>
        /// <param name="post">The post that is liked.</param>
        public Like(Guid id, User user, Post post)
        {
            Id = id;
            User = user;
            Post = post;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Like"/> class for a comment.
        /// </summary>
        /// <param name="id">The unique identifier for the like.</param>
        /// <param name="user">The user who gave the like.</param>
        /// <param name="comment">The comment that is liked.</param>
        public Like(Guid id, User user, Comment comment)
        {
            Id = id;
            User = user;
            Comment = comment;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current like.
        /// </summary>
        /// <param name="obj">The object to compare with the current like.</param>
        /// <returns>true if the specified object is equal to the current like; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Like like))
            {
                return false;
            }

            return like.User == User && like.Post == Post && like.Comment == Comment;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current like.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, User, Comment, Post);
        }
    }
}
