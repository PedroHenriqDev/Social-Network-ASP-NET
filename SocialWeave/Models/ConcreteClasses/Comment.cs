using System.ComponentModel.DataAnnotations;
using SocialWeave.Models.Interfaces;
using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class Comment : IFeedback
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(100, MinimumLength = 1)]
        public string Text {  get; set; }
        public IEnumerable<Like>? Likes { get; set; }
        public Post? Post {  get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        
        public Comment() 
        {
        }

        public Comment(Guid id, string text, User user, Post post)
        {
            Text = text;
        }
    }
}
