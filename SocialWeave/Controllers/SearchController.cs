using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Exceptions;
using SocialWeave.Helpers;
using SocialWeave.Models.Services;

namespace SocialWeave.Controllers
{
    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class SearchController : Controller
    {

        private readonly SearchService _searchService;
        private readonly UserService _userService;

        public SearchController(SearchService searchService, UserService userService)
        {
            _searchService = searchService;
            _userService = userService;
        }

        public IActionResult PageOfSearch()
        {
            return View();
        }

        public async Task<IActionResult> Search(string query)
        {
            try
            {
                ViewBag.Query = query;
                return View("PageOfSearch", await _searchService.SearchUsersAndPostsByQueryAsync(query, await _userService.FindUserByNameAsync(User.Identity.Name)));
            }
            catch (SearchException ex) 
            {
                return View(nameof(PageOfSearch));
            }
        }
    }
}
