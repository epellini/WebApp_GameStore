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
        public IActionResult ViewAdminPanel(string tab)
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                List<string> roles = (List<string>) _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    if (string.IsNullOrEmpty(tab))
                    {
                        tab = "Games";
                    }
                    ViewBag.View = tab;
                    AdminPanelViewModel adminPanelViewModel = new AdminPanelViewModel()
                    {
                        AllGames = _gameStoreManager.GetAllGames("Alphabetical"),
                        AllEvents = _gameStoreManager.GetAllEvents(),
                        AllReviews = _gameStoreManager.GetAllReviews()
                    };
                    return View("Panel", adminPanelViewModel);
                }
                
            }
            return View("AccessDenied");
        }

        // GET /admin/games from AJAX
        [HttpGet("/admin/games")]
        public JsonResult ViewGames()
        {
            List<Game> allGames = _gameStoreManager.GetAllGames("Alphabetical");
            return Json(new { games = JsonSerializer.Serialize(allGames) });
        }

        // GET /admin/reports/game-list
        [HttpGet("/admin/reports/game-list")]
        public IActionResult ViewGameListReport()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                List<string> roles = (List<string>) _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    List<Game> allGames = _gameStoreManager.GetAllGames("Alphabetical");
                    return View("ReportGameList", allGames);
                }
            }
            return View("AccessDenied");
        }

        // GET /admin/reports/game-details
        [HttpGet("/admin/reports/game-details")]
        public IActionResult ViewGameDetailsReport()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                List<string> roles = (List<string>)_userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    List<Game> allGames = _gameStoreManager.GetAllGames("Alphabetical");
                    return View("ReportGameDetails", allGames);
                }
            }
            return View("AccessDenied");
        }

        // GET /admin/reports/member-list
        [HttpGet("/admin/reports/member-list")]
        public IActionResult ViewMemberListReport()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                // Get list of all users
                List<User> allUsers = _userManager.Users.OrderBy(u => u.UserName).ToList();
                foreach (User u in allUsers)
                {
                    u.Profile = _gameStoreManager.GetProfileById(u.Id);
                }
                return View("ReportMemberList", allUsers);
            }
            return View("AccessDenied");
        }

        // GET /admin/reports/member-details
        [HttpGet("/admin/reports/member-details")]
        public IActionResult ViewMemberDetailsReport()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                // Get list of all users
                List<User> allUsers = _userManager.Users.OrderBy(u => u.UserName).ToList();
                foreach (User u in allUsers)
                {
                    u.Profile = _gameStoreManager.GetProfileById(u.Id);
                    u.WishedGames = _gameStoreManager.GetWishedGamesById(u.Id);
                    u.Friends = _gameStoreManager.GetFriendConnectsById(u.Id);
                }
                return View("ReportMemberDetails", allUsers);
            }
            return View("AccessDenied");
        }

        // GET /admin/reports/wishlist
        [HttpGet("/admin/reports/wishlist")]
        public IActionResult ViewWishlistReport()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                List<string> roles = (List<string>)_userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    List<Game> allGames = _gameStoreManager.GetAllGames("Alphabetical");
                    foreach (Game game in allGames)
                    {
                        game.WishedGames = _gameStoreManager.GetWishedGamesByGameId(game.GameId);
                    }
                    List<Game> wishedGames = allGames.Where(g => g.WishedGames.Count() > 0).ToList();
                    return View("ReportWishlist", wishedGames);
                }
            }
            return View("AccessDenied");
        }

        // GET /admin/reports/sales
        [HttpGet("/admin/reports/sales")]
        public IActionResult ViewSalesReport()
        {
            // Check if user has admin role:
            User user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                List<string> roles = (List<string>)_userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    List<Game> allGames = _gameStoreManager.GetAllGames("Alphabetical");
                    foreach (Game game in allGames)
                    {
                        game.OrderItems = _gameStoreManager.GetOrderItemsByGameId(game.GameId);
                    }
                    List<Game> soldGames = allGames.Where(g => g.OrderItems.Count() > 0).ToList();
                    return View("ReportSales", soldGames);
                }
            }
            return View("AccessDenied");
        }

        // GET /reviews/{id}/approve
        [HttpGet("/reviews/{id}/approve")]
        public IActionResult ApproveReview(int id)
        {
            // mark review status as "Approved"
            Review review =  _gameStoreManager.GetReviewById(id);
            if (review != null)
            {
                review.Status = "Approved";
                _gameStoreManager.UpdateReview(review);
            }
            return RedirectToAction("ViewAdminPanel", "Admin", new {tab = "Reviews"});
        }

        // GET /reviews/{id}/reject
        [HttpGet("/reviews/{id}/reject")]
        public IActionResult RejectReview(int id)
        {
            // mark review status as "Approved"
            Review review = _gameStoreManager.GetReviewById(id);
            if (review != null)
            {
                review.Status = "Rejected";
                _gameStoreManager.UpdateReview(review);
            }
            return RedirectToAction("ViewAdminPanel", "Admin", new { tab = "Reviews" });
        }

        // Private fields for services
        private IGameStoreManager _gameStoreManager;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
    }
}
