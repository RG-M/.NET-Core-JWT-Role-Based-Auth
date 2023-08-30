using Intranet.Entities;
using Intranet.IServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Intranet.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> addClaimToUser(AppUser appUser, string claimName, string claimValue)
        {
            return await userManager.AddClaimAsync(appUser, new Claim(claimName, claimValue));
        }

        public async Task<IdentityResult> AddRoleToUser(string userId, List<string> roles)
        {
            AppUser appUser = await userManager.FindByIdAsync(userId);
            return await userManager.AddToRolesAsync(appUser, roles);
        }

        public async Task<IdentityResult> removeRoleFromUser(string userId, List<string> roles)
        {
            AppUser appUser = await userManager.FindByIdAsync(userId);
            return await userManager.RemoveFromRolesAsync(appUser, roles);
        }
    }
}
