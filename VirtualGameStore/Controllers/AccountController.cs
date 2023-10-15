using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VirtualGameStore.Entities;
using VirtualGameStore.Models;

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

        // GET:
        // /account/signup
        [HttpGet("account/signup")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View("SignUp");
        }

        // POST:
        // /account/signup
        [HttpPost("account/signup")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // If the form is not valid, return the view with the model (showing errors, see RegisterViewModel):
            if (!ModelState.IsValid)
            {
                return View("Signup", model);
            }

            // If the form is valid create a new user with the given name and email:
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email
            };

            // Use the userManager to create a new user with the given password:
            // This adds the user to the AspNetUsers table
            // Save the result to a variable to check if the operation was successful:
            var result = await _userManager.CreateAsync(user, model.Password);

            // If the operation was not successful, add the errors to the model and return the view:
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                // This will display the error messages in the validation summary:
                return View("Signup", model);
            }

            // If the operation was successful, redirect to method that sends email:
            return RedirectToAction("SendVerification", new { email = user.Email });
        }

        [HttpGet("account/verify/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendVerification(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = user.Email }, Request.Scheme);
                string body = string.Format("<h2>Welcome to CVGS!</h2><hr>" +
                    $"<p><b>Hi {user.UserName.Substring(0, 1).ToUpper()}{user.UserName.Substring(1).ToLower()},</b></p><br>" +
                    $"<p>Thank you for signing up!</p>" +
                    $"<p>Click <a href=\"{confirmationLink}\">here</a> to verify your email address.</p>");
                await _emailSender.SendEmailAsync(user.Email, "Verify Your Email Address", body);
                return RedirectToAction("SuccessRegistration", "Account", new { email = user.Email });
            }

            ViewBag.errorMessage = "Unfortunately, we were unable to validate your account details.";
            return View("Error");
        }

        // GET:
        // /Account/SuccessRegistration
        [HttpGet("account/success")]
        [AllowAnonymous]
        public IActionResult SuccessRegistration(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return View("SuccessRegistration", email);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET:
        // /Account/ConfirmEmail
        [HttpGet("account/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return View("SuccessVerification");
                }
                else
                {
                    ViewBag.errorMessage = "Unfortunately, we were unable to process your email confirmation request. Please try again.";
                }
            }
            else
            {
                ViewBag.errorMessage = "Unfortunately, we were unable to validate your account details.";
            }
            return View("Error");
        }

        [HttpGet("account/login")]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new LoginViewModel();
            return View(model);
        }

        //POST:
        // /Account/LogIn
        [HttpPost("account/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            // If the form was filled out correctly:
            if (ModelState.IsValid)
            {
                // Try to find the user by username:
                var user = await _userManager.FindByNameAsync(model.Username);

                // If the user already exist:
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (!user.EmailConfirmed && !roles.Contains("Admin"))
                    {
                        return RedirectToAction("SuccessRegistration", new { email = user.Email });
                    }
                    if (_userManager.GetAccessFailedCountAsync(user).Result < 2)
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

                        if (result.Succeeded)
                        {
                            _userManager.ResetAccessFailedCountAsync(user);
                            return RedirectToAction("Index", "Home");
                        }
                        await _userManager.AccessFailedAsync(user);
                    }
                    else
                    {
                        if (!_userManager.IsLockedOutAsync(user).Result && !_userManager.IsLockedOutAsync(user).Result)
                        {
                            await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: true);
                        }
                        return View("Locked");
                    }

                }
            }

            ModelState.AddModelError("", "Invalid username/password.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
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

        [HttpGet("account/error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json("That email is registered to an account.");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CheckUsername(string username)
        {
            Regex emailrx = new Regex(@"(?i)^[a-z0-9]+$");
            if (!emailrx.IsMatch(username))
            {
                return Json("Username cannot have spaces or special characters.");
            }
            else
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return Json(true);
                }
                else
                {
                    return Json("That username is taken.");
                }
            }
        }

        // private fields for services
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
    }
}
