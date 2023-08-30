using Intranet.Data;
using Intranet.DTOs.Requests;
using Intranet.DTOs.Responses;
using Intranet.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Intranet.Security
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfig jwtConfig;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly IntranetDbContext intranetDbContext;
        private readonly UserManager<AppUser> userManager;



        public JwtService(IOptionsMonitor<JwtConfig> optionsMonitor,
                          TokenValidationParameters tokenValidationParameters,
                          IntranetDbContext intranetDbContext,
                          UserManager<AppUser> userManager
                          )
        {
            this.jwtConfig = optionsMonitor.CurrentValue;
            this.tokenValidationParameters = tokenValidationParameters;
            this.intranetDbContext = intranetDbContext;
            this.userManager = userManager;
        }

        public async Task<AuthResponse> generateJWT(IdentityUser user)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(jwtConfig.Key!);

                SigningCredentials signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor();
                tokenDescriptor.Subject = new ClaimsIdentity(new[]
               {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            });
                // the life span of the token needs to be shorter and utilise refresh token to keep the user signedin
                tokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(1);
                // here we are adding the encryption alogorithim information which will be used to decrypt our token
                tokenDescriptor.SigningCredentials = signingCredentials;

                var jwtTokenHandler = new JwtSecurityTokenHandler();

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);

                var jwtToken = jwtTokenHandler.WriteToken(token);
                RefreshToken refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsUsed = false,
                    UserId = user.Id,
                    CreatedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    IsRevoked = false,
                    Token = GenerateRandomToken()
                };

                await intranetDbContext.RefreshTokens.AddAsync(refreshToken);
                await intranetDbContext.SaveChangesAsync();

                return new AuthResponse(jwtToken, refreshToken.Token);


            }
            catch (Exception)
            {
                return new AuthResponse() { Error = "error" };
            }
        }

        public async Task<AuthResponse> VerifyAndGenerateNewJwtWithRefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal? claimsPrincipal = null;
            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(refreshTokenRequest.Token, tokenValidationParameters, out SecurityToken validatedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                // ignore expired date we will generate a new token for it
            }
            catch (Exception e)
            {
                return new AuthResponse() { Error = "error token not valid" + e.Message };
            }

            try
            {

                var utcExpiryDate = long.Parse(claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                DateTime expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).DateTime;
                var jti = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (expiryDate > DateTime.UtcNow)
                    return new AuthResponse() { Error = "We cannot refresh this since the token has not expired" };

                var storedRefreshToken = await intranetDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshTokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                    return new AuthResponse() { Error = "Wrefresh token doesnt exist" };

                if (storedRefreshToken.ExpiryDate < DateTime.UtcNow)
                    return new AuthResponse() { Error = "token has expired, user needs to relogin " };

                if (storedRefreshToken.IsUsed)
                    return new AuthResponse() { Error = "token has been used" };

                if (storedRefreshToken.IsRevoked)
                    return new AuthResponse() { Error = "token has been revoked" };


                if (storedRefreshToken.JwtId != jti)
                    return new AuthResponse() { Error = "the token doenst mateched the saved token" };


                storedRefreshToken.IsUsed = true;

                intranetDbContext.RefreshTokens.Update(storedRefreshToken);

                await intranetDbContext.SaveChangesAsync();

                var dbUser = await userManager.FindByIdAsync(storedRefreshToken.UserId);

                if (dbUser == null)
                    return new AuthResponse() { Error = "user not found" };


                return await generateJWT(dbUser);

            }
            catch (Exception)
            {
                return new AuthResponse() { Error = "error" };
            }

        }

        public string GenerateRandomToken(int length = 32)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
