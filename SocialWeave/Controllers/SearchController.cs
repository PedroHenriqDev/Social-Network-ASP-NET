using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using SocialWeave.Helpers;
using SocialWeave.Models.Services;

namespace SocialWeave.Controllers
{
    public class SearchController : Controller
    {

        private readonly SearchService _searchService;
        private readonly UserService _userService;
        private readonly AmountOfPostsHelper _amountOfPostsHelper;

        public SearchController(SearchService searchService, UserService userService, AmountOfPostsHelper amountOfPostsHelper)
        {
            _searchService = searchService;
            _userService = userService;
            _amountOfPostsHelper = amountOfPostsHelper;
        }
    }
}
