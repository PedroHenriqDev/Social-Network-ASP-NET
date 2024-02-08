using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using SocialWeave.Models.AbstractClasses;
using System.ComponentModel.DataAnnotations;
using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class Comment : Feedback
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "{0} is required!")] 
        public string Text {  get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public ICollection<Like> Likes { get; set; }
        
        public Comment() 
        {
        }

        public Comment(string text, DateTime date,User user)
        {
            Text = text;
            Date = date;
            User = user;
        }
    }
}
