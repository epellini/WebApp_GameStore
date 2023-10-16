using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
                            await _userManager.ResetAccessFailedCountAsync(user);
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
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("Unlock", "Account", new { token, email = user.Email }, Request.Scheme);
                        string body = string.Format("<h2>Unlock your account!</h2><hr>" +
                            $"<p><b>Hi {user.UserName.Substring(0, 1).ToUpper()}{user.UserName.Substring(1).ToLower()},</b></p><br>" +
                            $"<p>Your account has been locked due to too many log in attempts.</p>" +
                            $"<p>Click <a href=\"{confirmationLink}\">here</a> to unlock your account.</p>");
                        await _emailSender.SendEmailAsync(user.Email, "Your Account is Locked", body);
                        return View("Locked");
                    }

                }
            }

            ModelState.AddModelError("", "Invalid username/password.");
            return View(model);
        }

        // GET:
        // /Account/Unlock
        [HttpGet("account/unlock")]
        [AllowAnonymous]
        public async Task<IActionResult> Unlock(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    return View("SuccessUnlock");
                }
                else
                {
                    ViewBag.errorMessage = "Unfortunately, we were unable to unlock your account. Please try again.";
                }
            }
            else
            {
                ViewBag.errorMessage = "Unfortunately, we were unable to validate your account details.";
            }
            return View("Error");
        }

        [HttpGet("account/logout")]
        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        // Forgot Password
        [HttpGet("account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotViewModel();
            return View("ForgotPassword", model);
        }

        [HttpPost("account/forgot-password")]
        public async Task<IActionResult> SendForgotLink(ForgotViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email;
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var confirmationLink = Url.Action("ResetPasswordAttempt", "Account", new { token, email = user.Email }, Request.Scheme);
                    string body = string.Format("<h2>Forgot your password?</h2><hr>" +
                        $"<p><b>Hi {user.UserName.Substring(0, 1).ToUpper()}{user.UserName.Substring(1).ToLower()},</b></p><br>" +
                        $"<p>We received a request to reset your password!</p>" +
                        $"<p>Click <a href=\"{confirmationLink}\">here</a> to begin.</p>");
                    await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", body);
                }
                return View("ForgotVerification");
            }
            ModelState.AddModelError("", "Email address is invalid.");
            return View("ForgotPassword", model);
        }

        [HttpGet("account/reset-password-attempt")]
        public async Task<IActionResult> ResetPasswordAttempt(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, "Sesame123$");
                if (result.Succeeded)
                {
                    PasswordViewModel model = new PasswordViewModel();
                    model.Email = email;
                    return View("ResetPassword", model);
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

        [HttpGet("account/reset-password")]
        public async Task<IActionResult> ResetPassword(PasswordViewModel model)
        {
            
            return View(model);
        }

        [HttpPost("account/reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
        {
            // If the form is not valid, return the view with the model (showing errors, see RegisterViewModel):
            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            var result = await _userManager.ChangePasswordAsync(user, "Sesame123$", model.Password);

            // If the operation was not successful, add the errors to the model and return the view:
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                // This will display the error messages in the validation summary:
                return View("ResetPassword", model);
            }

            // If the operation was successful, redirect to method that sends email:
            return RedirectToAction("LogIn");
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
