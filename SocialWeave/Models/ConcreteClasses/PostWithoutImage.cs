using SocialWeave.Models.AbstractClasses;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class PostWithoutImage : Post
    {
        public PostWithoutImage()
        {
        }

        public PostWithoutImage(Guid id, [Required(ErrorMessage = "{0} is required")]
        [StringLength(50)]string description, DateTime date)
            : base(id, description, date)
        {

        }
    }
}