using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VirtualGameStore.Models
{
    public class ForgotViewModel
    {
        [Remote("CheckEmail", "Account")]
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }
    }
}
