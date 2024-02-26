using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.ViewModels
{
    /// <summary>
    /// ViewModel for the user page.
    /// </summary>
    public class UserPageViewModel
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Gets or sets the user associated with the page.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the count of users who admire the current user.
        /// </summary>
        public string CountAdmired { get; set; }

        /// <summary>
        /// Gets or sets the count of users whom the current user admires.
        /// </summary>
        public string CountAdmirer { get; set; }

        /// <summary>
        /// Gets or sets the comments associated with the user page.
        /// </summary>
        public IEnumerable<Comment> Comments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPageViewModel"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="user">The user associated with the page.</param>
        /// <param name="countAdmired">The count of users who admire the current user.</param>
        /// <param name="countAdmirer">The count of users whom the current user admires.</param>
        /// <param name="comments">The comments associated with the user page.</param>
        public UserPageViewModel(ApplicationDbContext context, User user, string countAdmired, string countAdmirer, IEnumerable<Comment> comments)
        {
            _context = context;
            User = user;
            CountAdmired = countAdmired;
            CountAdmirer = countAdmirer;
            Comments = comments;
        }

        /// <summary>
        /// Checks if the current user has admired another user.
        /// </summary>
        /// <param name="nameUser">The name of the user being admired.</param>
        /// <param name="nameCurrentUser">The name of the current user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, returning a boolean indicating whether the current user has admired another user.</returns>
        public async Task<bool> HaveAdmiredAsync(string nameUser, string nameCurrentUser)
        {
            if (nameUser == null || nameCurrentUser == null)
            {
                return false;
            }

            return await _context.Admirations.AnyAsync(x => x.UserAdmired.Name == nameUser && x.UserAdmirer.Name == nameCurrentUser);
        }
    }
}
