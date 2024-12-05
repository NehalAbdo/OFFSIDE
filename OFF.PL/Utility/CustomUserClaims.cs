using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OFF.DAL.Model;
using System.Security.Claims;

namespace OFF.PL.Utility
{
    public class CustomUserClaims : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public CustomUserClaims(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            return identity;
        }
    }

}
