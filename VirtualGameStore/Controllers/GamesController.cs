using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualGameStore.Entities;
using VirtualGameStore.Models;
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
            List<Platform> allPlatforms = _gameStoreManager.GetAllPlatforms();
            List<Genre> allGenres = _gameStoreManager.GetAllGenres();
            List<Language> allLanguages = _gameStoreManager.GetAllLanguages();

            EditGameViewModel editGameViewModel = new EditGameViewModel()
            {
                AllPlatforms = allPlatforms,
                AllGenres = allGenres,
                AllLanguages = allLanguages,

            };
            return View("AddGame", editGameViewModel);
        }

        // Post the game
        [HttpPost("games/add")]
        public async Task<IActionResult> SaveGame(EditGameViewModel editGameViewModel)
        {
            List<string> newPlats = new List<string>();
            if (!string.IsNullOrEmpty(editGameViewModel.Platforms))
            {
                editGameViewModel.GamePlatforms = new List<GamePlatform>();
                newPlats = editGameViewModel.Platforms.Split(";").Where(p => !string.IsNullOrEmpty(p)).ToList();
                foreach (string plat in newPlats)
                {
                    GamePlatform gamePlatform = new GamePlatform()
                    {
                        PlatformId = int.Parse(plat)
                    };
                    editGameViewModel.GamePlatforms.Add(gamePlatform);
                }
            }
            List<string> newGens = new List<string>();
            if (!string.IsNullOrEmpty(editGameViewModel.Genres))
            {
                editGameViewModel.GameGenres = new List<GameGenre>();
                newGens = editGameViewModel.Genres.Split(";").Where(p => !string.IsNullOrEmpty(p)).ToList();
                foreach(string gens in newGens)
                {
                    GameGenre gameGenre = new GameGenre()
                    {
                        GenreId = int.Parse(gens)
                    };
                    editGameViewModel.GameGenres.Add(gameGenre);
                }
            }
            List<string> newLangs = new List<string>();
            if (!string.IsNullOrEmpty(editGameViewModel.Languages))
            {
                editGameViewModel.GameLanguages = new List<GameLanguage>();
                newLangs = editGameViewModel.Languages.Split(";").Where(p => !string.IsNullOrEmpty(p)).ToList();
                foreach (var lang in newLangs)
                {
                    GameLanguage gameLanguage = new GameLanguage()
                    {
                        LanguageId = lang
                    };
                    editGameViewModel.GameLanguages.Add(gameLanguage);
                }
            }
            if (ModelState.IsValid)
            {
                Game game = new Game();

                // Assign game properties
                game.Name = editGameViewModel.Name;
                game.Description = editGameViewModel.Description;
                game.Developer = editGameViewModel.Developer;
                game.ReleaseDate = editGameViewModel.ReleaseDate;
                game.RetailPrice = editGameViewModel.RetailPrice;

                if (editGameViewModel.NewPicture != null)
                {
                    Picture picture = new Picture()
                    {
                        GameId = game.GameId,
                        Game = game,
                        AltText = "Cover image for " + game.Name,
                        IsCoverArt = true
                    };
                    _gameStoreManager.CreatePicture(editGameViewModel.NewPicture, picture);
                }

                // Create game in the database
                _gameStoreManager.CreateGame(game);

                // Link genres, languages, and platforms to the game
                foreach (GameGenre gen in editGameViewModel.GameGenres)
                {
                    gen.GameId = game.GameId;
                    _gameStoreManager.CreateGameGenre(gen);
                }
                foreach (GameLanguage lang in editGameViewModel.GameLanguages)
                {
                    lang.GameId = game.GameId;
                    _gameStoreManager.CreateGameLanguage(lang);
                }
                foreach (GamePlatform plat in editGameViewModel.GamePlatforms)
                {
                    plat.GameId = game.GameId;
                    _gameStoreManager.CreateGamePlatform(plat);
                }

                // Redirect to game list
                return RedirectToAction("ViewAllGames");
            }
            editGameViewModel.AllGenres = _gameStoreManager.GetAllGenres();
            editGameViewModel.AllLanguages = _gameStoreManager.GetAllLanguages();
            editGameViewModel.AllPlatforms = _gameStoreManager.GetAllPlatforms();
            return View("AddGame", editGameViewModel);
        }

        // GET: /images/{id}
        [HttpGet("images/{id}")]
        public IActionResult ViewImage(int id)
        {
            Picture? picture = _gameStoreManager.GetPictureById(id);
            if (picture != null)
            {
                return File(picture.Image, "image/jpg");
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost("/account/upload-game-cover")]
        public JsonResult UploadPhoto(EditGameViewModel editGameViewModel)
        {
            Game? game = _gameStoreManager.GetGameById(editGameViewModel.GameId);

            Picture picture = new Picture()
            {

            };

            //_gameStoreManager.CreatePicture(editProfileViewModel.NewPhoto, photo);
            return Json(new { pictureId = picture.PictureId });
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
