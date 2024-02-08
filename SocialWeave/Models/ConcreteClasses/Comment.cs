using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class Comment : IFeedback
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "{0} is required!")] 
        public string Text {  get; set; }

        public DateTime Date { get; set; }
        
        public Comment() 
        {
        }

        public Comment(string text, DateTime date,User user)
        {
            Text = text;
            Date = date;
        }
    }
}
