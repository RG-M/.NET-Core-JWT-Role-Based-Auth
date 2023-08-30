using Intranet.Entities;
using Intranet.IServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Intranet.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> addClaimToRole(string roleName, string claimName, string claimValue)
        {
            IdentityRole role = await roleManager.FindByNameAsync(roleName);
            return await roleManager.AddClaimAsync(role, new Claim(claimName,claimValue));
        }

        
    }
}
