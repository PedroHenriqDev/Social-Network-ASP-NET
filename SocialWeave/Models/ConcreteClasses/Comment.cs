using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Comment : IComment
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "{0} is required!")] 
        public string Text {  get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Dislike> Dislikes { get; set; }

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
