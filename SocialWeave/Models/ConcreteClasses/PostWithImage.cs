using SocialWeave.Models.AbstractClasses;
using System;

namespace SocialWeave.Models.ConcreteClasses
{
    /// <summary>
    /// Represents a post with an image in the system.
    /// </summary>
    public sealed class PostWithImage : Post
    {
        /// <summary>
        /// The byte array representing the image associated with the post.
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Default constructor for the PostWithImage class.
        /// </summary>
        public PostWithImage()
        {
        }

        /// <summary>
        /// Parameterized constructor for the PostWithImage class.
        /// </summary>
        /// <param name="id">The unique identifier for the post.</param>
        /// <param name="description">The description of the post.</param>
        /// <param name="date">The date of creation of the post.</param>
        /// <param name="image">The byte array representing the image associated with the post.</param>
        public PostWithImage(Guid id, string description, DateTime date, byte[] image)
            : base(id, description, date)
        {
            Image = image;
        }

        /// <summary>
        /// Converts the image byte array to a base64-encoded string.
        /// </summary>
        /// <returns>The base64-encoded string representation of the image.</returns>
        public string ConvertImageToBase64()
        {
            return Convert.ToBase64String(Image);
        }
    }
}
