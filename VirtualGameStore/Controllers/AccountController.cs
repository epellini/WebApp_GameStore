using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace VirtualGameStore.Controllers
{
    public class AccountController : Controller
    {
       
      
        
        public IActionResult Login()
        {
            return View();
        }


        // Register/Signup
        [HttpGet]
        public IActionResult Register()
        {
            return View("SignUp");
        }

        // Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        //Registration Success page
        //It's going to be shown to the user after email confirmation
        [HttpGet]
        public IActionResult Success()
        {
            return View("Success");
        }

        //Locked
        [HttpGet]
        public IActionResult Locked()
        {
            return View("Locked");
        }
    }
}
