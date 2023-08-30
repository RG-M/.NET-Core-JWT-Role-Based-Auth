using Intranet.DTOs.Requests;
using Intranet.DTOs.Responses;
using Intranet.Entities;
using Intranet.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IJwtService jwtService;

        public AuthController(UserManager<AppUser> userManager, IJwtService jwtService)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto user)
        {
            var userExist = await userManager.FindByEmailAsync(user.Email);
            if (userExist != null)
                return BadRequest(new AuthResponse() { Error = "Email Exist" });
            

            AppUser newUser = new AppUser()
            {
                Email = user.Email,
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            IdentityResult identityRes = await userManager.CreateAsync(newUser, user.Password);
            if (identityRes.Succeeded)
                return Ok(await jwtService.generateJWT(newUser));

            else
                return BadRequest(new AuthResponse() { Error = "Error," + identityRes.Errors.Select(x => x.Description).ToList() });

        }


        

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            AppUser appUser = await userManager.FindByEmailAsync(userLoginRequest.Email);

            if(appUser == null)
                return Unauthorized("Email Not Exist");

            bool checkPassword = await userManager.CheckPasswordAsync(appUser, userLoginRequest.Password);

            if (!checkPassword)
                return Unauthorized("Password Incorrect");

            return Ok(await jwtService.generateJWT(appUser));

        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            AuthResponse resp = await jwtService.VerifyAndGenerateNewJwtWithRefreshToken(refreshTokenRequest);
            return resp.Error !=null ? BadRequest(resp) : Ok(resp);

        }


    }
}



