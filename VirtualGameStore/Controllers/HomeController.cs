using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VirtualGameStore.Entities;
using VirtualGameStore.Models;
using VirtualGameStore.Services;

namespace VirtualGameStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IGameStoreManager gameStoreManager, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _gameStoreManager = gameStoreManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ViewAllGames", "Games");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Private service interface field (must be to the interface not derived class due to scoped service):
        private IGameStoreManager _gameStoreManager;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
    }
}