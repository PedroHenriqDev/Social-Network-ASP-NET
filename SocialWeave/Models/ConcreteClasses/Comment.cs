using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Comment : IComment
    {
        public Guid Id { get; set; }
        public string Text {  get; set; }
        public int Like {  get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Dislike> Dislikes { get; set; }

        public Comment() 
        {
        }

        public Comment(string text, int like, DateTime date,User user)
        {
            Text = text;
            Like = like;
            Date = date;
            User = user;
        }
    }
}
