using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(100, MinimumLength = 1)]
        public string Text { get; set; }
    }
}
