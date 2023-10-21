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
        public IActionResult ViewAllGames(string sort)
        {
            if (string.IsNullOrEmpty(sort))
            {
                sort = "New";
            }
            List<Game> allGames = _gameStoreManager.GetAllGames(sort).ToList();
            ViewBag.Sort = sort;
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

        [HttpGet("games/search")]
        public JsonResult SearchGames(string query)
        {
            List<Game> games = _gameStoreManager.GetGamesBySearch(query);
            List<Platform>[]? platforms = new List<Platform>[games.Count];
            List<Picture>[]? pictures = new List<Picture>[games.Count];

            for (int i = 0; i < games.Count; i++)
            {
                List<GamePlatform> gamePlatforms = games[i].Platforms.ToList();
                platforms[i] = new List<Platform>();
                foreach (var gamePlatform in gamePlatforms)
                {
                    gamePlatform.Platform.Games = null;
                    platforms[i].Add(gamePlatform.Platform);
                }
                pictures[i] = games[i].Pictures.ToList();
                games[i].Platforms = null;
                games[i].Pictures = null;
            }
            string sort = "";
            if (string.IsNullOrEmpty(query))
            {
                sort = "New";
            }
            return Json(new { games = games, platforms = platforms, pictures = pictures, sort = sort});
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
    }
}
