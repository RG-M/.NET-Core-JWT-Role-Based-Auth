using Intranet.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Intranet.IServices
{
    public interface IUserService
    {
        public Task<IdentityResult> addClaimToUser(AppUser appUser, string claimName, string claimValue);

        public Task<IdentityResult> AddRoleToUser(string userId, List<string> roles);
        public Task<IdentityResult> removeRoleFromUser(string userId, List<string> roles);

    }
}
