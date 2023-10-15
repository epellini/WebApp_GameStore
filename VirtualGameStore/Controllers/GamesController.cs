using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualGameStore.Entities;
using VirtualGameStore.Services;

namespace VirtualGameStore.Controllers
{
    public class GamesController : Controller
    {
        // Constructor to assign services to private fields
        public GamesController(IGameStoreManager gameStoreManager, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _gameStoreManager = gameStoreManager;
            _signInManager = signInManager;
            _userManager = userManager;

        }

        // GET: /games
        [HttpGet("games")]
        public IActionResult ViewAllGames()
        {
            List<Game> allGames = _gameStoreManager.GetAllGames().ToList();
            return View("AllGames", allGames);
        }

        // GET: /images/{id}
        [HttpGet("images/{id}")]
        public IActionResult ViewImage(int id)
        {
            Picture picture = _gameStoreManager.GetPictureById(id);
            if (picture != null)
            {
                return File(picture.Image, "image/jpg");
            }
            return RedirectToAction("Error", "Home");
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
    }
}
