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
        public async Task<IActionResult> ViewAllGames(string sort)
        {
            if (string.IsNullOrEmpty(sort))
            {
                sort = "New";
            }
            List<Game> allGames = _gameStoreManager.GetAllGames(sort).ToList();
            ViewBag.Sort = sort;
            AllGamesViewModel viewModel = new AllGamesViewModel()
            {
                Games = allGames,
                WishedGames = new List<WishedGame>()
            };
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    List<WishedGame>? wishes = _gameStoreManager.GetWishedGamesById(user.Id);
                    if (wishes != null)
                    {
                        viewModel.WishedGames = wishes;
                    }
                }
            }

            return View("AllGames", viewModel);
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
                foreach (string gens in newGens)
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
                return RedirectToAction("ViewAdminPanel", "Admin");
            }
            editGameViewModel.AllGenres = _gameStoreManager.GetAllGenres();
            editGameViewModel.AllLanguages = _gameStoreManager.GetAllLanguages();
            editGameViewModel.AllPlatforms = _gameStoreManager.GetAllPlatforms();
            return View("AddGame", editGameViewModel);
        }

        // GET: /games/{gameId}/edit
        [HttpGet("games/{gameId}/edit-game")]
        public IActionResult EditGame(int gameId)
        {
            Game game = _gameStoreManager.GetGameById(gameId);

            string platformNames = "";
            foreach (GamePlatform platform in game.Platforms)
            {
                platformNames += platform.PlatformId + ";";
            }
            string genresNames = "";
            foreach (GameGenre genre in game.Genres)
            {
                genresNames += genre.GenreId + ";";
            }
            string languageNames = "";
            foreach (GameLanguage language in game.Languages)
            {
                languageNames += language.LanguageId + ";";
            }

            List<Platform> allPlatforms = _gameStoreManager.GetAllPlatforms();
            List<Genre> allGenres = _gameStoreManager.GetAllGenres();
            List<Language> allLanguages = _gameStoreManager.GetAllLanguages();

            EditGameViewModel editGameViewModel = new EditGameViewModel()
            {
                GameId = gameId,
                Name = game.Name,
                Description = game.Description,
                Developer = game.Developer,
                ReleaseDate = game.ReleaseDate,
                RetailPrice = game.RetailPrice,
                AllPlatforms = allPlatforms,
                AllGenres = allGenres,
                AllLanguages = allLanguages,
                GameGenres = game.Genres.ToList(),
                GameLanguages = game.Languages.ToList(),
                GamePlatforms = game.Platforms.ToList(),
                Platforms = platformNames,
                Genres = genresNames,
                Languages = languageNames,
                CurrentPicture = game.Pictures.Where(p => p.IsCoverArt == true).FirstOrDefault()
            };
            return View("EditGame", editGameViewModel);
        }

        [HttpPost("games/{gameId}/edit-game")]
        public async Task<IActionResult> UpdateGame(EditGameViewModel editGameViewModel)
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
                foreach (string gens in newGens)
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
                // Create "old" lists of preferences to test against
                Game game = _gameStoreManager.GetGameById(editGameViewModel.GameId);

                game.Name = editGameViewModel.Name;
                game.Description = editGameViewModel.Description;
                game.Developer = editGameViewModel.Developer;
                game.ReleaseDate = editGameViewModel.ReleaseDate;
                game.RetailPrice = editGameViewModel.RetailPrice;

                // Remove any platforms that were just unselected (in old list but not in new list):
                if (game.Platforms != null)
                {
                    // Loop through the game's old list of platforms
                    foreach (var plat in game.Platforms)
                    {
                        // Try to find a match from the new list
                        if (newPlats.Count > 0)
                        {
                            string s = newPlats.Where(p => p.Equals(plat.PlatformId.ToString())).FirstOrDefault();

                            // If no match, the platform should be deleted
                            if (s == null)
                            {
                                _gameStoreManager.DeleteGamePlatform(plat);
                            }
                        }
                        // If the new list has no platforms, all old ones should be deleted
                        else
                        {
                            _gameStoreManager.DeleteGamePlatform(plat);
                        }
                    }
                }

                // Add any platforms that were just selected (in new list but not in old list):
                if (newPlats != null)
                {
                    // Loop through the new list of favourite platforms
                    foreach (var plat in newPlats)
                    {
                        // Try to find a match from the old list
                        GamePlatform gp = game.Platforms.Where(p => p.PlatformId.ToString() == plat).FirstOrDefault();
                        if (gp == null)
                        {
                            Platform p = _gameStoreManager.GetAllPlatforms().Where(a => a.PlatformId.ToString() == plat).FirstOrDefault();

                            // If no match, the platform should be added (created)
                            if (p != null)
                            {
                                gp = new GamePlatform()
                                {
                                    GameId = game.GameId,
                                    PlatformId = p.PlatformId,
                                };
                                _gameStoreManager.CreateGamePlatform(gp);
                            }
                        }
                    }
                }

                // Remove any genres that were just unselected (in old list but not in new list):
                if (game.Genres != null)
                {
                    // Loop through the game's old list of genres
                    foreach (var gen in game.Genres)
                    {
                        // Try to find a match from the new list
                        if (newGens.Count > 0)
                        {
                            string s = newGens.Where(p => p.Equals(gen.GenreId.ToString())).FirstOrDefault();

                            // If no match, the genre should be deleted
                            if (s == null)
                            {
                                _gameStoreManager.DeleteGameGenre(gen);
                            }
                        }
                        // If the new list has no genres, all old ones should be deleted
                        else
                        {
                            _gameStoreManager.DeleteGameGenre(gen);
                        }
                    }
                }

                // Add any genres that were just selected (in new list but not in old list):
                if (newGens != null)
                {
                    // Loop through the new list of favourite genres
                    foreach (var gen in newGens)
                    {
                        // Try to find a match from the old list
                        GameGenre gg = game.Genres.Where(fg => fg.GenreId.ToString() == gen).FirstOrDefault();
                        if (gg == null)
                        {
                            Genre g = _gameStoreManager.GetAllGenres().Where(a => a.GenreId.ToString() == gen).FirstOrDefault();

                            // If no match, the genre should be added (created)
                            if (g != null)
                            {
                                gg = new GameGenre()
                                {
                                    GameId = game.GameId,
                                    GenreId = g.GenreId,
                                };
                                _gameStoreManager.CreateGameGenre(gg);
                            }
                        }
                    }
                }

                // Remove any languages that were just deselected (in old list but not in new list):
                if (game.Languages != null)
                {
                    // Loop through the games's old list of languages
                    foreach (var lang in game.Languages)
                    {
                        // Try to find a match from the new list
                        if (newLangs.Count > 0)
                        {
                            string s = newLangs.Where(l => l.Equals(lang.LanguageId)).FirstOrDefault();

                            // If no match, the language should be deselected (deleted)
                            if (s == null)
                            {
                                _gameStoreManager.DeleteGameLanguage(lang);
                            }
                        }
                        // If the new list has no languages, all old ones should be deleted
                        else
                        {
                            _gameStoreManager.DeleteGameLanguage(lang);
                        }
                    }
                }

                // Add any languages that were just selected (in new list but not in old list):
                if (newLangs != null)
                {
                    // Loop through the new list of languages
                    foreach (var lang in newLangs)
                    {
                        // Try to find a match from the old list
                        GameLanguage gl = game.Languages.Where(pl => pl.LanguageId == lang).FirstOrDefault();
                        if (gl == null)
                        {
                            Language l = _gameStoreManager.GetAllLanguages().Where(a => a.LanguageId == lang).FirstOrDefault();

                            // If no match, the languge should be added (created)
                            if (l != null)
                            {
                                gl = new GameLanguage()
                                {
                                    GameId = game.GameId,
                                    LanguageId = l.LanguageId,
                                };
                                _gameStoreManager.CreateGameLanguage(gl);
                            }
                        }
                    }
                }

                _gameStoreManager.UpdateGame(game);

                return RedirectToAction("ViewAdminPanel", "Admin");
            }
            editGameViewModel.AllGenres = _gameStoreManager.GetAllGenres();
            editGameViewModel.AllLanguages = _gameStoreManager.GetAllLanguages();
            editGameViewModel.AllPlatforms = _gameStoreManager.GetAllPlatforms();
            return View("EditGame", editGameViewModel);
        }

        // GET: /games/{gameId}/delete
        [HttpGet("games/{gameId}/delete")]
        public IActionResult DeleteGame(int gameId)
        {
            Game game = _gameStoreManager.GetGameById(gameId);

            if (game != null)
            {
                _gameStoreManager.DeleteGame(game);
                return RedirectToAction("ViewAdminPanel", "Admin");
            }
            else
            {
                ViewBag.errorMessage = "Game not found.";
            }
            return View("Error", "Account");
        }

        // GET: /games/{id}
        [HttpGet("games/{id}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = _gameStoreManager.GetGameById(id);

            if (game != null)
            {
                GameDetailsViewModel gameDetailsViewModel = new GameDetailsViewModel
                {
                    Game = game,
                    Name = game.Name,
                    GameId = game.GameId,
                    Description = game.Description,
                    Developer = game.Developer,
                    Pictures = game.Pictures.ToList(),
                    ReleaseDate = game.ReleaseDate,
                    RetailPrice = game.RetailPrice,
                    Genres = new List<Genre>(),
                    Platforms = new List<Platform>(),
                    Languages = new List<Language>(),
                    Reviews = _gameStoreManager.GetAllReviewsByGameId(id),
                    AvgRating = 0.0,
                    IsSignedIn = false
                };

                List<Rating>? ratings = _gameStoreManager.GetAllRatingsByGameId(id);
                if (ratings != null && ratings.Count() > 0)
                {
                    int total = 0;
                    foreach (Rating r in ratings)
                    {
                        total += r.RatingValue;
                    }
                    gameDetailsViewModel.AvgRating = (double)total / (double)ratings.Count();
                }

                if (game.Genres != null)
                {
                    foreach (var genre in game.Genres)
                    {
                        if (genre.Genre != null)
                        {
                            gameDetailsViewModel.Genres.Add(genre.Genre);
                        }
                    }
                    foreach (var platform in game.Platforms)
                    {
                        if (platform.Platform != null)
                        {
                            gameDetailsViewModel.Platforms.Add(platform.Platform);
                        }
                    }
                    foreach (var language in game.Languages)
                    {
                        if (language.Language != null)
                        {
                            gameDetailsViewModel.Languages.Add(language.Language);
                        }
                    }
                }
                if (_signInManager.IsSignedIn(User))
                {
                    User user = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (user != null)
                    {
                        gameDetailsViewModel.IsSignedIn = true;
                        List<WishedGame>? wishes = _gameStoreManager.GetWishedGamesById(user.Id);
                        if (wishes != null)
                        {
                            gameDetailsViewModel.Wishlist = wishes;
                        }
                        gameDetailsViewModel.NewReview = new Review()
                        {
                            UserId = user.Id,
                            GameId = id,
                            Game = game
                        };
                        gameDetailsViewModel.PendingReviewCount = _gameStoreManager.GetAllReviewsByUserId(user.Id).Where(r => r.Status == "Pending").Count();
                        if (ratings != null)
                        {
                            Rating? rating = ratings.Where(r => r.UserId == user.Id).FirstOrDefault();
                            if (rating != null)
                            {
                                gameDetailsViewModel.Rating = rating.RatingValue;
                            }
                        }
                    }
                }

                return View("Game", gameDetailsViewModel);
            }
            else
            {
                ViewBag.errorMessage = "Game not found.";
            }
            return View("Error", "Account");
        }

        [HttpPost("games/{id}/add-to-wishlist")]
        public JsonResult AddToWishlist(int id)
        {
            bool loggedIn = false;
            bool added = false;

            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    loggedIn = true;
                    Game game = _gameStoreManager.GetGameById(id);
                    if (game != null)
                    {
                        user.WishedGames = _gameStoreManager.GetWishedGamesById(user.Id);
                        bool alreadyWished = false;
                        if (user.WishedGames != null && user.WishedGames.Count > 0)
                        {
                            foreach (var wishedGame in user.WishedGames)
                            {
                                if (wishedGame.GameId == id)
                                {
                                    _gameStoreManager.DeleteWishedGame(wishedGame);
                                    alreadyWished = true;
                                    break;
                                }
                            }
                        }
                        if (!alreadyWished)
                        {
                            WishedGame wish = new WishedGame()
                            {
                                GameId = game.GameId,
                                UserId = user.Id,
                            };

                            _gameStoreManager.CreateWishedGame(wish);
                        }
                        added = true;
                    }
                }
            }
            return Json(new { loggedIn = loggedIn, added = added, id = id });
        }

        // Add rating jsonresult
        [HttpPost("games/{id}/rate/{rate}")]
        public JsonResult RateGame(int id, int rate)
        {
            bool loggedIn = false;
            bool added = false;
            string average = "0.0";

            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    loggedIn = true;
                    Game game = _gameStoreManager.GetGameById(id);
                    if (game != null)
                    {
                        Rating? existingRate = _gameStoreManager.GetAllRatingsByGameId(id).Where(r => r.UserId == user.Id).FirstOrDefault();
                        if (existingRate != null)
                        {
                            existingRate.RatingValue = rate;
                            _gameStoreManager.UpdateRating(existingRate);
                            added = true;
                        }
                        else
                        {
                            Rating newRating = new Rating()
                            {
                                GameId = id,
                                UserId = user.Id,
                                RatingValue = rate
                            };
                            _gameStoreManager.CreateRating(newRating);
                            added = true;
                        }
                        List<Rating>? ratings = _gameStoreManager.GetAllRatingsByGameId(id);
                        if (ratings != null)
                        {
                            int total = 0;
                            foreach (Rating r in ratings)
                            {
                                total += r.RatingValue;
                            }
                            average = ((double)total / (double)ratings.Count()).ToString("0.0#");
                        }
                    }
                }
            }
            return Json(new { loggedIn = loggedIn, added = added, id = id, average = average });
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

        [HttpPost("/games/upload-game-cover")]
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
            return Json(new { games = games, platforms = platforms, pictures = pictures, sort = sort });
        }

        // Post method to submit a review of a game
        [HttpPost("/games/{id}")]
        public async Task<IActionResult> SubmitReview(int id, GameDetailsViewModel gameDetailsViewModel)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    Game game = _gameStoreManager.GetGameById(id);
                    if (game != null)
                    {
                        Review review = new Review()
                        {
                            GameId = game.GameId,
                            UserId = user.Id,
                            ReviewText = gameDetailsViewModel.NewReview.ReviewText,
                            ReviewDate = DateTime.Now
                        };
                        _gameStoreManager.CreateReview(review);
                        return RedirectToAction("GetGameById", "Games", new { id = id });
                    }
                    else
                    {
                        ViewBag.errorMessage = "Game not found.";
                    }
                    return View("Error", "Account");
                }
            }
            return RedirectToAction("GetGameById", "Games", new {id = id});
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
    }
}
