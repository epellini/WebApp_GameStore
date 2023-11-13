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

        //adding game through the admin panel
        // GET: /games/add
        [HttpGet("games/add")]
        public IActionResult AddGame()
        {
            ViewBag.Genres = _gameStoreManager.GetAllGenres().ToList();
            ViewBag.Languages = _gameStoreManager.GetAllLanguages().ToList();
            ViewBag.Platforms = _gameStoreManager.GetAllPlatforms().ToList();
            return View("AddGame", new Game());
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



        // Post the game
        [HttpPost]
        public async Task<IActionResult> SaveGame(Game game, IFormFile picture, int[] Genres, int[] Languages, int[] Platforms)
        {
            if (ModelState.IsValid)
            {
                // Process and save game cover photo
                if (picture != null && picture.Length > 0)
                {
                    byte[] imageData = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        await picture.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }

                    var gamePicture = new Picture { Image = imageData };
                    // Assuming you need to add this picture to a collection in the Game entity
                    game.Pictures = new List<Picture> { gamePicture };
                }

                // Link genres, languages, and platforms to the game
                game.Genres = Genres.Select(genreId => new GameGenre { GameId = game.GameId, GenreId = genreId }).ToList();
                game.Languages = Languages.Select(languageId => new GameLanguage { GameId = game.GameId, LanguageId = languageId.ToString() }).ToList();
                game.Platforms = Platforms.Select(platformId => new GamePlatform { GameId = game.GameId, PlatformId = platformId }).ToList();

                // Create game in the database
                _gameStoreManager.CreateGame(game);

                // Redirect to game list
                return RedirectToAction("ViewAllGames");
            }
            return View("AddGame", game);
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

    }



}
