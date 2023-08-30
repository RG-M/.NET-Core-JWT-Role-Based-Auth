using Intranet.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Intranet.IServices
{
    public interface IRoleService
    {
        public Task<IdentityResult> addClaimToRole(string roleName, string claimName, string claimValue);

    }
}
