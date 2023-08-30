using Intranet.DTOs.Requests;
using Intranet.Entities;
using Intranet.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [AllowAnonymous]
    [Route("users")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUserService userService;
        public UsersController(UserManager<AppUser> userManager,IUserService userService)
        {
            this.userManager = userManager;
            this.userService = userService;
        }


        [HttpPost]
        [Route("addRoles")]
        public async Task<IdentityResult> AddRoleToUser([FromBody] UserRoleRequest userRoleRequest)
        {
            return await userService.AddRoleToUser(userRoleRequest.userId, userRoleRequest.roles);
        }

        [HttpPost]
        [Route("removeRoles")]
        public async Task<IdentityResult> removeRoleFromUser([FromBody] UserRoleRequest userRoleRequest)
        {
            return await userService.removeRoleFromUser(userRoleRequest.userId, userRoleRequest.roles);
        }
    }
}
