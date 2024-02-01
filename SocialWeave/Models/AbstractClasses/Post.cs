using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.AbstractClasses
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public List<Comment> comments {  get; set; }
    }
}
