using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using VirtualGameStore.Entities;

namespace VirtualGameStore.Controllers
{
    public class AccountController : Controller
    {
       // Constructor to assign services to private fields
      public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // TODO: remove when email is confirmed working for login/signup
        // Temporary POST method for home page to test email service
        [HttpPost]
        public async Task<IActionResult> TestEmail()
        {
            // I'm using HttpContext Request to quickly get the form data
            // but we will want to use view models for the actual application:
            string email = HttpContext.Request.Form["email"];

            // Create a link to imbed in the email
            string? link = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            // Create an email body in HTML (including the link)
            string body = string.Format("<h2>Hey punk!</h2><hr>" +
                    $"<p><b>Thanks for giving me your email address</b></p><br>" +
                    $"<p>Now I'm going to spam you with ads for boner pills and stuff!</p>" +
                    $"<p>Click <a href=\"{link}\">here</a> if you wanna get hacked</p>");

            // Send the email
            await _emailSender.SendEmailAsync(email, "1 weird coding trick! Developers hate him", body);
            return RedirectToAction("Success");
        }

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

        // private fields for services
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
    }
}
