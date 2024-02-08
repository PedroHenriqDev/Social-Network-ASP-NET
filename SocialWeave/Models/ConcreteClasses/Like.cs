using Microsoft.Identity.Client;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.Interfaces;

namespace SocialWeave.Models.ConcreteClasses
{
    sealed public class Like : IFeedback
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Comment? Comment { get; set;}
        public Post? Post { get; set; }

        public Like(Guid id, User user, Post post)
        {
            Id = id;
            User = user;
            Post = post;
        }

        public Like(Guid id, User user, Comment comment)
        {
            Id = id;
            User = user;
            Comment = comment;
        }

        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }

            Like like = obj as Like;
            return like.User == User && like.Post == Post;
        }

    }
}
