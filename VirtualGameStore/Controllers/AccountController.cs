using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VirtualGameStore.Entities;
using VirtualGameStore.Models;
using VirtualGameStore.Services;

namespace VirtualGameStore.Controllers
{
    public class AccountController : Controller
    {
        // Constructor to assign services to private fields
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IGameStoreManager gameStoreManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _gameStoreManager = gameStoreManager;
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
                    Profile profile = new Profile
                    {
                        UserId = user.Id,
                        User = user,
                        JoinDate = DateTime.Today
                    };
                    _gameStoreManager.CreateProfile(profile);
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

            // If the operation was successful, redirect to login:
            return RedirectToAction("Login");
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

        [HttpGet("/users/{username}")]
        public async Task<IActionResult> ViewProfile(string username)
        {
            User user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                Profile profile = _gameStoreManager.GetProfileById(user.Id);

                List<WishedGame>? wishes = _gameStoreManager.GetWishedGamesById(user.Id);

                List<FriendConnect>? friends = _gameStoreManager.GetFriendConnectsById(user.Id);

                if (profile == null)
                {
                    profile = new Profile
                    {
                        UserId = user.Id,
                        User = user,
                        JoinDate = DateTime.Today
                    };
                    _gameStoreManager.CreateProfile(profile);
                }

                ProfileViewModel profileViewModel = new ProfileViewModel()
                {
                    User = user,
                    Profile = profile,
                    IsOwner = false,
                    IsSignedIn = false,
                    WishedGames = wishes,
                    Friends = friends
                };

                if (_signInManager.IsSignedIn(User))
                {
                    profileViewModel.IsSignedIn = true;
                    User curUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
                    if (curUser == user)
                    {
                        profileViewModel.IsOwner = true;
                    }
                    else
                    {
                        if (profileViewModel.Friends != null)
                        {
                            profileViewModel.ExistingFriend = profileViewModel.Friends.Where(f => f.FriendId == curUser.Id).FirstOrDefault();
                        }
                    }
                }
                ViewBag.Section = "Wishlist";
                return View("Profile", profileViewModel);
            }
            else
            {
                ViewBag.errorMessage = "Account not found.";
            }
            return View("Error");
        }

        [HttpGet("/wished-games/{id}/unwish")]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    WishedGame? wish = _gameStoreManager.GetWishedGame(id);
                    if (wish != null)
                    {
                        _gameStoreManager.DeleteWishedGame(wish);
                        return RedirectToAction("ViewProfile", new { username = wish.User.UserName });
                    }
                    else
                    {
                        ViewBag.errorMessage = "Wishlisted item not found.";
                    }
                    return View("Error");
                }
            }

            return RedirectToAction("LogIn", "Account");
        }

        [HttpGet("account/friend-requests/new-request/{id}")]
        public IActionResult SendFriendRequest(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (_signInManager.IsSignedIn(User))
                {
                    User user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                    FriendConnect testConnect = _gameStoreManager.GetFriendConnectsById(user.Id).Where(f => f.FriendId == id).FirstOrDefault();

                    if (testConnect == null)
                    {
                        User friend = _userManager.FindByIdAsync(id).Result;
                        if (friend != null)
                        {
                            FriendConnect friendConnect = new FriendConnect()
                            {
                                UserId = user.Id,
                                User = user,
                                FriendId = friend.Id,
                                Friend = friend,
                                Status = "Pending",
                                DateConnected = DateTime.Now
                            };
                            FriendConnect friendRequest = new FriendConnect()
                            {
                                UserId = friend.Id,
                                User = friend,
                                FriendId = user.Id,
                                Friend = user,
                                Status = "Requested",
                                DateConnected = DateTime.Now
                            };
                            _gameStoreManager.CreateFriendConnect(friendConnect);
                            _gameStoreManager.CreateFriendConnect(friendRequest);
                            return RedirectToAction("ViewProfile", new { username = friend.UserName });
                        }
                        else
                        {
                            ViewBag.errorMessage = "User not found.";
                        }
                    }
                    else
                    {
                        ViewBag.errorMessage = "Friend request already sent.";
                    }

                    return View("Error");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("account/friend-requests/{id}/accept")]
        public IActionResult AcceptFriendRequest(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                FriendConnect? friend = _gameStoreManager.GetFriendConnect(id);
                if (friend != null)
                {
                    if (user.Id == friend.UserId)
                    {
                        FriendConnect? connect = _gameStoreManager.GetFriendConnectsById(friend.FriendId).Where(f => f.FriendId == friend.UserId).FirstOrDefault();

                        if (connect != null)
                        {
                            friend.Status = "Confirmed";
                            connect.Status = "Confirmed";
                            _gameStoreManager.UpdateFriendConnect(friend);
                            _gameStoreManager.UpdateFriendConnect(connect);
                            return RedirectToAction("ViewProfile", new { username = friend.User.UserName });
                        }
                    }
                    else
                    {
                        ViewBag.errorMessage = "Access Denied. You do not have permission do make changes to this friend request.";
                    }
                }
                else
                {
                    ViewBag.errorMessage = "Friend request not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet("account/friend-requests/{id}/confirm")]
        public IActionResult ConfirmFriendRequest(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                FriendConnect? friend = _gameStoreManager.GetFriendConnect(id);
                if (friend != null)
                {
                    if (user.Id == friend.FriendId)
                    {
                        FriendConnect? connect = _gameStoreManager.GetFriendConnectsById(user.Id).Where(f => f.FriendId == friend.UserId).FirstOrDefault();

                        if (connect != null)
                        {
                            friend.Status = "Confirmed";
                            connect.Status = "Confirmed";
                            _gameStoreManager.UpdateFriendConnect(friend);
                            _gameStoreManager.UpdateFriendConnect(connect);
                            return RedirectToAction("ViewProfile", new { username = friend.User.UserName });
                        }
                    }
                    else
                    {
                        ViewBag.errorMessage = "Access Denied. You do not have permission do make changes to this friend request.";
                    }
                }
                else
                {
                    ViewBag.errorMessage = "Friend request not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("account/friend-requests/{id}/delete")]
        public IActionResult RemoveFriendRequest(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                FriendConnect? friend = _gameStoreManager.GetFriendConnect(id);
                if (friend != null)
                {
                    if (user.Id == friend.UserId)
                    {
                        FriendConnect? connect = _gameStoreManager.GetFriendConnectsById(friend.FriendId).Where(f => f.FriendId == friend.UserId).FirstOrDefault();

                        if (connect != null)
                        {
                            _gameStoreManager.DeleteFriendConnect(friend);
                            _gameStoreManager.DeleteFriendConnect(connect);
                            return RedirectToAction("ViewProfile", new { username = friend.User.UserName });
                        }
                    }
                    else
                    {
                        ViewBag.errorMessage = "Access Denied. You do not have permission do make changes to this friend request.";
                    }
                }
                else
                {
                    ViewBag.errorMessage = "Friend request not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("account/friend-requests/{id}/remove")]
        public IActionResult CancelFriendRequest(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                FriendConnect? friend = _gameStoreManager.GetFriendConnect(id);
                if (friend != null)
                {
                    if (user.Id == friend.FriendId)
                    {
                        FriendConnect? connect = _gameStoreManager.GetFriendConnectsById(friend.FriendId).Where(f => f.FriendId == friend.UserId).FirstOrDefault();

                        if (connect != null)
                        {
                            _gameStoreManager.DeleteFriendConnect(friend);
                            _gameStoreManager.DeleteFriendConnect(connect);
                            return RedirectToAction("ViewProfile", new { username = friend.User.UserName });
                        }
                    }
                    else
                    {
                        ViewBag.errorMessage = "Access Denied. You do not have permission do make changes to this friend request.";
                    }
                }
                else
                {
                    ViewBag.errorMessage = "Friend request not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/account/edit-profile")]
        public async Task<IActionResult> EditProfile()
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                Profile profile = _gameStoreManager.GetProfileById(user.Id);
                if (profile == null)
                {
                    profile = new Profile();
                }
                EditProfileViewModel editProfileViewModel = new EditProfileViewModel()
                {
                    ProfileId = profile.ProfileId,
                    UserId = user.Id,
                    Username = user.UserName,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Gender = profile.Gender,
                    BirthDate = profile.BirthDate,
                    PromoRegistered = profile.PromoRegistered,
                    CurrentPhoto = profile.Photos.Where(p => p.isProfilePic == true).FirstOrDefault(),
                };

                return View("EditProfile", editProfileViewModel);
            }

            return RedirectToAction("Login");

        }

        [HttpPost("/account/edit-profile")]
        public async Task<IActionResult> SaveProfile(EditProfileViewModel editProfileViewModel)
        {
            // Get the image file from the request form keys
            //IFormFile file = Request.Form.Keys.FirstOrDefault();

            if (ModelState.IsValid)
            {
                User user = _userManager.FindByIdAsync(editProfileViewModel.UserId).Result;
                if (user != null)
                {
                    Profile existingProfile = _gameStoreManager.GetProfileById(user.Id);
                    existingProfile.Gender = editProfileViewModel.Gender;
                    existingProfile.BirthDate = editProfileViewModel.BirthDate;
                    existingProfile.FirstName = editProfileViewModel.FirstName;
                    existingProfile.LastName = editProfileViewModel.LastName;
                    existingProfile.PromoRegistered = editProfileViewModel.PromoRegistered;
                    if (editProfileViewModel.NewPhoto != null)
                    {
                        foreach (Photo oldPhoto in existingProfile.Photos)
                        {
                            oldPhoto.isProfilePic = false;
                        }
                        Photo photo = new Photo()
                        {
                            ProfileId = existingProfile.ProfileId,
                            Profile = existingProfile,
                            AltText = "Profile photo for " + user.UserName,
                            isProfilePic = true
                        };
                        _gameStoreManager.CreatePhoto(editProfileViewModel.NewPhoto, photo);
                    }

                    _gameStoreManager.UpdateProfile(existingProfile);
                    return RedirectToAction("ViewProfile", new { user.UserName });
                }
                else
                {
                    ViewBag.errorMessage = "Unfortunately, we were unable to make the changes you requested.";
                }
                return View("Error");
            }
            return View("EditProfile", editProfileViewModel);
        }

        // GET: /images/user-uploaded/{photoId}
        [HttpGet("/images/user-uploaded/{photoId}")]
        public IActionResult ViewPhoto(int photoId)
        {
            Photo? photo = _gameStoreManager.GetPhotoById(photoId);
            if (photo != null)
            {
                return File(photo.Image, "image/jpg");
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost("/account/upload-photo")]
        public JsonResult UploadPhoto(EditProfileViewModel editProfileViewModel)
        {
            User user = _userManager.FindByIdAsync(editProfileViewModel.UserId).Result;
            Profile profile = _gameStoreManager.GetProfileById(editProfileViewModel.UserId);

            Photo photo = new Photo()
            {
                ProfileId = editProfileViewModel.ProfileId,
                Profile = profile,
                AltText = "Profile photo for " + user.UserName,
                isProfilePic = false
            };

            _gameStoreManager.CreatePhoto(editProfileViewModel.NewPhoto, photo);
            return Json(new { photoId = photo.PhotoId });
        }

        [HttpGet("/account/preferences")]
        public async Task<IActionResult> ViewPreferences()
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                user.Platforms = _gameStoreManager.GetFavouritePlatformsById(user.Id);
                string platformNames = "";
                foreach (FavouritePlatform platform in user.Platforms)
                {
                    platformNames += platform.Platform.PlatformName + ";";
                }
                user.Genres = _gameStoreManager.GetFavouriteGenresById(user.Id);
                string genresNames = "";
                foreach (FavouriteGenre genre in user.Genres)
                {
                    genresNames += genre.Genre.GenreName + ";";
                }
                user.Languages = _gameStoreManager.GetPreferredLanguagesById(user.Id);
                string languageNames = "";
                foreach (PreferredLanguage language in user.Languages)
                {
                    languageNames += language.Language.LanguageName + ";";
                }

                List<Platform> allPlatforms = _gameStoreManager.GetAllPlatforms();
                List<Genre> allGenres = _gameStoreManager.GetAllGenres();
                List<Language> allLanguages = _gameStoreManager.GetAllLanguages();

                PreferencesViewModel preferenceViewModel = new PreferencesViewModel()
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    AllPlatforms = allPlatforms,
                    AllGenres = allGenres,
                    AllLanguages = allLanguages,
                    FavPlatforms = user.Platforms.ToList(),
                    FavGenres = user.Genres.ToList(),
                    PrefLanguages = user.Languages.ToList(),
                    Platforms = platformNames,
                    Genres = genresNames,
                    Languages = languageNames
                };

                return View("Preferences", preferenceViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("/account/preferences")]
        public async Task<IActionResult> SavePreferences(PreferencesViewModel preferenceViewModel)
        {

            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                // Create "old" lists of preferences to test against
                List<FavouritePlatform> oldPlats = _gameStoreManager.GetFavouritePlatformsById(user.Id);
                List<FavouriteGenre> oldGens = _gameStoreManager.GetFavouriteGenresById(user.Id);
                List<PreferredLanguage> oldLangs = _gameStoreManager.GetPreferredLanguagesById(user.Id);

                // Create "new" lists of preferences from the post request
                List<string> newPlats = new List<string>();
                if (!string.IsNullOrEmpty(preferenceViewModel.Platforms))
                {
                    newPlats = preferenceViewModel.Platforms.Split(";").Where(p => !string.IsNullOrEmpty(p)).ToList();
                }
                List<string> newGens = new List<string>();
                if (!string.IsNullOrEmpty(preferenceViewModel.Genres))
                {
                    newGens = preferenceViewModel.Genres.Split(";").Where(p => !string.IsNullOrEmpty(p)).ToList();
                }
                List<string> newLangs = new List<string>();
                if (!string.IsNullOrEmpty(preferenceViewModel.Languages))
                {
                    newLangs = preferenceViewModel.Languages.Split(";").Where(p => !string.IsNullOrEmpty(p)).ToList();
                }

                // Remove any platforms that were just unfavourited (in old list but not in new list):
                if (oldPlats != null)
                {
                    // Loop through the user's old list of favourite platforms
                    foreach (var plat in oldPlats)
                    {
                        // Try to find a match from the new list
                        if (newPlats.Count > 0)
                        {
                            string s = newPlats.Where(p => p.Equals(plat.Platform.PlatformName)).FirstOrDefault();

                            // If no match, the platform should be unfavourited (deleted)
                            if (s == null)
                            {
                                _gameStoreManager.DeleteFavouritePlatform(plat);
                            }
                        }
                        // If the new list has no platforms, all old ones should be deleted
                        else
                        {
                            _gameStoreManager.DeleteFavouritePlatform(plat);
                        }
                    }
                }

                // Add any platforms that were just favourited (in new list but not in old list):
                if (newPlats != null)
                {
                    // Loop through the new list of favourite platforms
                    foreach (var plat in newPlats)
                    {
                        // Try to find a match from the old list
                        FavouritePlatform fp = oldPlats.Where(fp => fp.Platform.PlatformName == plat).FirstOrDefault();
                        if (fp == null)
                        {
                            Platform p = _gameStoreManager.GetAllPlatforms().Where(a => a.PlatformName == plat).FirstOrDefault();

                            // If no match, the platform should be added (created)
                            if (p != null)
                            {
                                fp = new FavouritePlatform()
                                {
                                    UserId = user.Id,
                                    PlatformId = p.PlatformId,
                                };
                                _gameStoreManager.CreateFavouritePlatform(fp);
                            }
                        }
                    }
                }

                // Remove any genres that were just unfavourited (in old list but not in new list):
                if (oldGens != null)
                {
                    // Loop through the user's old list of favourite genres
                    foreach (var gen in oldGens)
                    {
                        // Try to find a match from the new list
                        if (newGens.Count > 0)
                        {
                            string s = newGens.Where(p => p.Equals(gen.Genre.GenreName)).FirstOrDefault();

                            // If no match, the genre should be unfavourited (deleted)
                            if (s == null)
                            {
                                _gameStoreManager.DeleteFavouriteGenre(gen);
                            }
                        }
                        // If the new list has no genres, all old ones should be deleted
                        else
                        {
                            _gameStoreManager.DeleteFavouriteGenre(gen);
                        }
                    }
                }

                // Add any genres that were just favourited (in new list but not in old list):
                if (newGens != null)
                {
                    // Loop through the new list of favourite genres
                    foreach (var gen in newGens)
                    {
                        // Try to find a match from the old list
                        FavouriteGenre fg = oldGens.Where(fg => fg.Genre.GenreName == gen).FirstOrDefault();
                        if (fg == null)
                        {
                            Genre g = _gameStoreManager.GetAllGenres().Where(a => a.GenreName == gen).FirstOrDefault();

                            // If no match, the genre should be added (created)
                            if (g != null)
                            {
                                fg = new FavouriteGenre()
                                {
                                    UserId = user.Id,
                                    GenreId = g.GenreId,
                                };
                                _gameStoreManager.CreateFavouriteGenre(fg);
                            }
                        }
                    }
                }

                // Remove any languages that were just deselected (in old list but not in new list):
                if (oldLangs != null)
                {
                    // Loop through the user's old list of preferred languages
                    foreach (var lang in oldLangs)
                    {
                        // Try to find a match from the new list
                        if (newLangs.Count > 0)
                        {
                            string s = newLangs.Where(l => l.Equals(lang.Language.LanguageName)).FirstOrDefault();

                            // If no match, the language should be deselected (deleted)
                            if (s == null)
                            {
                                _gameStoreManager.DeletePreferredLanguage(lang);
                            }
                        }
                        // If the new list has no languages, all old ones should be deleted
                        else
                        {
                            _gameStoreManager.DeletePreferredLanguage(lang);
                        }
                    }
                }

                // Add any languages that were just selected (in new list but not in old list):
                if (newLangs != null)
                {
                    // Loop through the new list of preferred languages
                    foreach (var lang in newLangs)
                    {
                        // Try to find a match from the old list
                        PreferredLanguage pl = oldLangs.Where(pl => pl.Language.LanguageName == lang).FirstOrDefault();
                        if (pl == null)
                        {
                            Language l = _gameStoreManager.GetAllLanguages().Where(a => a.LanguageName == lang).FirstOrDefault();

                            // If no match, the languge should be added (created)
                            if (l != null)
                            {
                                pl = new PreferredLanguage()
                                {
                                    UserId = user.Id,
                                    LanguageId = l.LanguageId,
                                };
                                _gameStoreManager.CreatePreferredLanguage(pl);
                            }
                        }
                    }
                }

                return RedirectToAction("ViewPreferences");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/account/addresses")]
        public async Task<IActionResult> ViewAddresses()
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                user.ShippingAddresses = _gameStoreManager.GetShippingAddressesById(user.Id);

                AddressViewModel addressViewModel = new AddressViewModel()
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    ShippingAddresses = user.ShippingAddresses.ToList()
                };
                return View("Addresses", addressViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/account/addresses/{addressId}")]
        public async Task<IActionResult> EditAddress(int addressId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                ShippingAddress shippingAddress = _gameStoreManager.GetAddressById(addressId);
                if (shippingAddress != null)
                {
                    return View("EditAddress", shippingAddress);
                }
                else
                {
                    ViewBag.errorMessage = "Address not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("/account/addresses/{addressId}")]
        public async Task<IActionResult> SaveAddress(ShippingAddress shippingAddress)
        {
            if (ModelState.IsValid)
            {
                _gameStoreManager.UpdateShippingAddress(shippingAddress);
                return RedirectToAction("ViewAddresses");
            }
            return View("EditAddress", shippingAddress);
        }

        [HttpGet("/account/addresses/new-address")]
        public async Task<IActionResult> NewAddress()
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                ShippingAddress shippingAddress = new ShippingAddress()
                {
                    UserId = user.Id,
                    User = user
                };
                return View("NewAddress", shippingAddress);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("/account/addresses/new-address")]
        public async Task<IActionResult> SaveNewAddress(ShippingAddress shippingAddress)
        {
            if (ModelState.IsValid)
            {
                _gameStoreManager.CreateShippingAddress(shippingAddress);
                return RedirectToAction("ViewAddresses");
            }
            return View("NewAddress", shippingAddress);
        }

        [HttpGet("/account/addresses/{addressId}/set-default")]
        public async Task<IActionResult> UpdateDefaultAddress(int addressId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                ShippingAddress shippingAddress = _gameStoreManager.GetAddressById(addressId);
                List<ShippingAddress> allAddresses = _gameStoreManager.GetShippingAddressesById(user.Id);
                if (shippingAddress != null)
                {
                    foreach (var a in allAddresses)
                    {
                        a.IsDefault = false;
                        _gameStoreManager.UpdateShippingAddress(a);
                    }
                    shippingAddress.IsDefault = true;
                    _gameStoreManager.UpdateShippingAddress(shippingAddress);
                    return RedirectToAction("ViewAddresses");
                }
                else
                {
                    ViewBag.errorMessage = "Address not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/account/addresses/{addressId}/delete-address")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                ShippingAddress shippingAddress = _gameStoreManager.GetAddressById(addressId);
                if (shippingAddress != null)
                {
                    _gameStoreManager.DeleteShippingAddress(shippingAddress);
                    return RedirectToAction("ViewAddresses");
                }
                else
                {
                    ViewBag.errorMessage = "Address not found.";
                }
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> CheckPhoneNumber(string phone)
        {
            Regex emailrx = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
            if (!emailrx.IsMatch(phone))
            {
                return Json("Phone number is not valid.");
            }
            else
            {
                return Json(true);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CheckAddress(string address)
        {
            Regex emailrx = new Regex(@"^[A-Za-z0-9]*\-?[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$");
            if (!emailrx.IsMatch(address))
            {
                return Json("Address is not valid.");
            }
            else
            {
                return Json(true);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CheckPostalCode(string postalCode)
        {
            Regex emailrx = new Regex(@"^([a-zA-Z]\d[a-zA-Z]( )?\d[a-zA-Z]\d)$");
            if (!emailrx.IsMatch(postalCode))
            {
                return Json("Postal code is not valid.");
            }
            else
            {
                return Json(true);
            }
        }

        [HttpGet("/account/cart")]
        public async Task<IActionResult> ViewCart()
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                Cart? cart = _gameStoreManager.GetCartById(user.Id);
                if (cart != null)
                {
                    List<Game> games = new List<Game>();
                    foreach (var item in cart.Items)
                    {
                        Game? game = _gameStoreManager.GetGameById(item.GameId);
                        if (game != null)
                        {
                            games.Add(game);
                        }
                    }
                    CartViewModel cartViewModel = new CartViewModel()
                    {
                        ShoppingCart = cart,
                        shoppingCartGames = games,
                        UserId = user.Id,
                        ShippingAddresses = _gameStoreManager.GetShippingAddressesById(user.Id)
                    };
                    return View("Cart", cartViewModel);
                }
                cart = new Cart()
                {
                    UserId = user.Id
                };
                CartViewModel newCartViewModel = new CartViewModel()
                {
                    ShoppingCart = cart,
                    UserId = user.Id,
                    shoppingCartGames = new List<Game>(),
                    ShippingAddresses = _gameStoreManager.GetShippingAddressesById(user.Id)
                };
                return View("Cart", newCartViewModel);
            }
            return RedirectToAction("ViewAllGames", "Games");
        }

        [HttpGet("games/{id}/add-to-cart")]
        public IActionResult AddGameToCart(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    Cart? cart = _gameStoreManager.GetCartById(user.Id);
                    if (cart != null)
                    {
                        CartItem cartItem = new CartItem()
                        {
                            CartId = cart.CartId,
                            GameId = id
                        };
                        _gameStoreManager.AddItemToCart(cartItem);
                    }
                    else
                    {
                        Cart newCart = new Cart()
                        {
                            UserId = user.Id
                        };
                        _gameStoreManager.CreateCart(newCart);
                        CartItem cartItem = new CartItem()
                        {
                            CartId = newCart.CartId,
                            GameId = id
                        };
                        _gameStoreManager.AddItemToCart(cartItem);
                    }
                }
            }
            return RedirectToAction("ViewAllGames", "Games");
        }

        [HttpGet("account/cart/item/{id}/remove")]
        public IActionResult RemoveGameFromCart(int id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    Cart? cart = _gameStoreManager.GetCartById(user.Id);
                    if (cart != null)
                    {
                        CartItem cartItem = _gameStoreManager.GetCartItemById(id);
                        _gameStoreManager.RemoveItemFromCart(cartItem);
                    }
                }
            }
            return RedirectToAction("ViewCart", "Account");
        }

        // Purchase cart
        [HttpPost("account/cart")]
        public async Task<IActionResult> PurchaseCart(CartViewModel cartViewModel)
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Cart? cart = _gameStoreManager.GetCartById(user.Id);
            if (cart != null)
            {
                List<Game> games = new List<Game>();
                foreach (var item in cart.Items)
                {
                    Game? game = _gameStoreManager.GetGameById(item.GameId);
                    if (game != null)
                    {
                        games.Add(game);
                    }
                }
                cartViewModel.ShoppingCart = cart;
                cartViewModel.shoppingCartGames = games;
                cartViewModel.UserId = user.Id;
                cartViewModel.ShippingAddresses = _gameStoreManager.GetShippingAddressesById(user.Id);
            }
            if (ModelState.IsValid)
            {
                decimal subtotal = 0;
                decimal tax = 0;
                decimal total = 0;
                foreach (Game game in cartViewModel.shoppingCartGames)
                {
                    subtotal += (decimal)game.RetailPrice;
                    tax += (decimal)game.RetailPrice * 0.13m;
                    total += (decimal)game.RetailPrice * 1.13m;
                }
                ShippingAddress? shippingAddress = _gameStoreManager.GetAddressById((int)cartViewModel.AddressId);
                Order newOrder = new Order()
                {
                    UserId = user.Id,
                    ShippingAddressId = (int)cartViewModel.AddressId,
                    OrderDate = DateTime.Now,
                    Subtotal = subtotal,
                    Tax = tax,
                    Total = total,
                    Status = "Paid",
                    BillingName = shippingAddress.FullName
                };
                _gameStoreManager.CreateOrder(newOrder);

                foreach (var item in cart.Items)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        GameId = item.GameId,
                        OrderId = newOrder.OrderId
                    };
                    _gameStoreManager.CreateOrderItem(orderItem);
                }
                foreach (var item in cart.Items)
                {
                    _gameStoreManager.RemoveItemFromCart(item);
                }
                return RedirectToAction("ViewDownloads", "Account");
            }
            return View("Cart", cartViewModel);
        }

        // Get view downloads:
        [HttpGet("/account/downloads")]
        public async Task<IActionResult> ViewDownloads()
        {
            if (_signInManager.IsSignedIn(User))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                List<OrderItem> items = _gameStoreManager.GetOrderItemsById(user.Id);
                return View("Downloads", items);
            }
            return RedirectToAction("ViewAllGames", "Games");
        }


        // private fields for services
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IGameStoreManager _gameStoreManager;
        private IEmailSender _emailSender;
    }
}
