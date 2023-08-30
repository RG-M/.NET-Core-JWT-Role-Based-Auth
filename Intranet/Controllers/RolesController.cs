using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [AllowAnonymous]
    [Route("roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IdentityResult> AddRole([FromBody] string roleName)
        {
            return await roleManager.CreateAsync(new IdentityRole(roleName));
        }


    }
}
