using SocialWeave.Models.ConcreteClasses;
using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.AbstractClasses
{
    abstract public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public DateTime Date { get; set; }
        public List<Comment> Comments {  get; set; }
        public ICollection<Like> Like { get; set; }
        public ICollection<Dislike> Dislikes { get; set; }
        public User User { get; set; }

        public Post() 
        {
        }

        public Post(Guid id, string description, DateTime date) 
        {
            Id = id;
            Description = description;
            Date = date;
        }
    }
}
