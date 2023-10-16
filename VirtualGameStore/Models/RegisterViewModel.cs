using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Models
{
    public class RegisterViewModel
    {
        [Remote("CheckEmail", "Account")]
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Remote("CheckUsername", "Account")]
        [Required(ErrorMessage = "Please enter a username.")]
        [StringLength(25, ErrorMessage = "Username cannot be more than 25 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [StringLength(48, ErrorMessage = "Password cannot be more than 48 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string? ConfirmPassword { get; set; }
    }
}
