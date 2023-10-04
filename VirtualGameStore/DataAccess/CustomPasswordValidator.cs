using Microsoft.AspNetCore.Identity;
using VirtualGameStore.Entities;

namespace VirtualGameStore.DataAccess
{
    internal class CustomPasswordValidator : PasswordValidator<User>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<User> userManager, User user, string password)
        {
            return IdentityResult.Success;
        }
    }
}
