using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class PostWithImage : Post
    {
        public byte[] Image { get; set; }

        public PostWithImage() 
        {
        }

        public PostWithImage(Guid id, string description, DateTime date, byte[] image) 
            : base(id, description, date)
        {
            Image = image;
        }
    }
}
