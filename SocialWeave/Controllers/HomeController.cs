using Microsoft.AspNetCore.Mvc;
using SocialWeave.Models.Services;
using SocialWeave.Models.ViewModels;
using System.Diagnostics;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        
        public HomeController(UserService userService) 
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
