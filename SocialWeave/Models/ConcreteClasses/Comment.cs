using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text {  get; set; }
        public int Like {  get; set; }
        public DateTime Date { get; set; }
        public User user { get; set; }
        public Post post { get; set; }

        public Comment() 
        {

        }

        public Comment(string text, int like, DateTime date, User user, Post post)
        {
            
        }
        
    }
}
