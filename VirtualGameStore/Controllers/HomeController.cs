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

        public HomeController(ILogger<HomeController> logger, IGameStoreManager gameStoreManager)
        {
            _logger = logger;
            _gameStoreManager = gameStoreManager;
        }

        public IActionResult Index()
        {
            List<Game> games = _gameStoreManager.GetAllGames().ToList();
            return View(games);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Private service interface field (must be to the interface not derived class due to scoped service):
        private IGameStoreManager _gameStoreManager;
    }
}