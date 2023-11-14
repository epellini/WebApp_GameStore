using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VirtualGameStore.Entities;
using VirtualGameStore.Models;
using VirtualGameStore.Services;

namespace VirtualGameStore.Controllers
{
    public class AdminController : Controller
    {
        // Constructor to assign services to private fields
        public AdminController(IGameStoreManager gameStoreManager, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _gameStoreManager = gameStoreManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET /admin
        [HttpGet("/admin")]
        public IActionResult ViewAdminPanel()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                List<string> roles = (List<string>) _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    ViewBag.View = "Games";
                    AdminPanelViewModel adminPanelViewModel = new AdminPanelViewModel()
                    {
                        AllGames = _gameStoreManager.GetAllGames("Alphabetical")
                    };
                    return View("Panel", adminPanelViewModel);
                }
                
            }
            return View("AccessDenied");
        }

        [HttpGet("/admin/games")]
        public JsonResult ViewGames()
        {
            List<Game> allGames = _gameStoreManager.GetAllGames("Alphabetical");
            return Json(new { games = JsonSerializer.Serialize(allGames) });
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
    }
}
