using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Models
{
    public class PasswordViewModel
    {
        [Required(ErrorMessage = "Please enter a password.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [StringLength(48, ErrorMessage = "Password cannot be more than 48 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string? ConfirmPassword { get; set; }

        public string Email { get; set; }
    }
}
