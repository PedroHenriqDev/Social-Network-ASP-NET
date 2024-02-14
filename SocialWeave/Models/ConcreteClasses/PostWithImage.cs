using SocialWeave.Models.AbstractClasses;
using System.Buffers.Text;
using System.Reflection.Metadata.Ecma335;

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

        public string ConvertImageToBase64() 
        {
            return Convert.ToBase64String(Image);
        }
    }
}
