using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.AbstractClasses
{
    abstract public class Post<T>
    {
        abstract public T Id { get; set; }
        abstract public T Description { get; set; }
        abstract public T Date { get; set; }
        abstract public List<T> Comments {  get; set; }

        public Post() 
        {
        }

        public Post(T id, T description, T date) 
        {
            Id = id;
            Description = description;
            Date = date;
        }
    }
}
