using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;
using System.Collections.Generic;

namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// ViewModel for search results.
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>
        /// Gets or sets the list of users matching the search criteria.
        /// </summary>
        public IEnumerable<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the list of posts matching the search criteria.
        /// </summary>
        public IEnumerable<Post> Posts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
        /// </summary>
        /// <param name="users">The list of users matching the search criteria.</param>
        /// <param name="posts">The list of posts matching the search criteria.</param>
        public SearchViewModel(IEnumerable<User> users, IEnumerable<Post> posts)
        {
            Users = users;
            Posts = posts;
        }
    }
}
