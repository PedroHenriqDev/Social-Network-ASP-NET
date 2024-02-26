using System;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// View model for creating a comment.
    /// </summary>
    public class CommentViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the post to which the comment belongs.
        /// </summary>
        [Required(ErrorMessage = "PostId is required.")]
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets or sets the text content of the comment.
        /// </summary>
        [Required(ErrorMessage = "Text is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The length of Text must be between 1 and 100 characters.")]
        public string Text { get; set; }
    }
}
