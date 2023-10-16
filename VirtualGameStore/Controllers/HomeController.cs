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

        [HttpGet("/users/{username}")]
		public async Task<IActionResult> Profile(string username)
		{
            User user = await _userManager.FindByNameAsync(username);
            user.Profile = _gameStoreManager.GetProfileById(user.Id);

			return View(user);
		}

        [HttpGet("/users/{username}/preferences")]
        public async Task<IActionResult> Preferences(string username)
        {
            User prefUser = await _userManager.FindByNameAsync(username);
            if (_signInManager.IsSignedIn(User))
            {
                if (User.Identity.Name == prefUser.UserName)
                {
                    prefUser.Platforms = _gameStoreManager.GetFavouritePlatformById(prefUser.Id);
                    prefUser.Genres = _gameStoreManager.GetFavouriteGenreById(prefUser.Id);
                    prefUser.Languages = _gameStoreManager.GetPreferredLanguagesById(prefUser.Id);
                    prefUser.ShippingAddresses = _gameStoreManager.GetShippingAddressesById(prefUser.Id);

                    return View(prefUser);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet("/users/{username}/edit-preferences")]
        public async Task<IActionResult> EditPreferences(string username)
        {
            User prefUser = await _userManager.FindByNameAsync(username);
            if (_signInManager.IsSignedIn(User))
            {
                if (User.Identity.Name == prefUser.UserName)
                {
                    prefUser.Platforms = _gameStoreManager.GetFavouritePlatformById(prefUser.Id);
                    prefUser.Genres = _gameStoreManager.GetFavouriteGenreById(prefUser.Id);
                    prefUser.Languages = _gameStoreManager.GetPreferredLanguagesById(prefUser.Id);
                    prefUser.ShippingAddresses = _gameStoreManager.GetShippingAddressesById(prefUser.Id);

                    return View(prefUser);
                }
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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