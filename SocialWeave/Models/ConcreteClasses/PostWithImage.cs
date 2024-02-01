using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class PostWithImage : Post
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public List<Comment> Comments { get; set; }

        public PostWithImage() 
        {
        }

        public PostWithImage(Guid id, string description, string date) 
        {
            Id = id;
            Description = description;
            Date = date;
        }
    }
}
