using SocialWeave.Models.AbstractClasses;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class PostWithoutImage : Post
    {
       public PostWithoutImage() 
       {
       }

        public PostWithoutImage(Guid id, string description, DateTime date) 
            : base(id, description, date)
        {
           
        }
    }
}