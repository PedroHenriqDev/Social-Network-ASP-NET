using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Helpers;
using SocialWeave.Models.Services;
using System.Threading.Tasks;

namespace SocialWeave.Controllers
{
    /// <summary>
    /// Controller responsible for handling search functionality.
    /// </summary>
    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="searchService">The search service used for searching users and posts.</param>
        /// <param name="userService">The user service used for user-related operations.</param>
        public SearchController(SearchService searchService, UserService userService)
        {
            _searchService = searchService;
            _userService = userService;
        }

        /// <summary>
        /// Displays the page for performing searches.
        /// </summary>
        /// <returns>The view for the search page.</returns>
        public IActionResult PageOfSearch()
        {
            return View();
        }

        /// <summary>
        /// Handles the search request based on the provided query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>The view displaying search results based on the query.</returns>
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                ViewBag.Query = query; // Set the query in ViewBag for access in the view
                // Search users and posts based on the query and the currently logged-in user
                ViewData["CurrentUser"] = await _userService.FindUserByNameAsync(User.Identity.Name);
                var resultOfQuery = await _searchService.SearchUsersAndPostsByQueryAsync(query, await _userService.FindUserByNameAsync(User.Identity.Name));
                return View("PageOfSearch", resultOfQuery);
            }
            catch (SearchException ex)
            {
                // In case of a search exception, return to the search page without displaying results
                return View(nameof(PageOfSearch));
            }
        }
    }
}
