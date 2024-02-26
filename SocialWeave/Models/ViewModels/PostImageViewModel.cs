namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// View model for representing a post with an image.
    /// </summary>
    public class PostImageViewModel
    {
        /// <summary>
        /// Gets or sets the description of the post.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the byte array representing the image of the post.
        /// </summary>
        public byte[] Image { get; set; }
    }
}
