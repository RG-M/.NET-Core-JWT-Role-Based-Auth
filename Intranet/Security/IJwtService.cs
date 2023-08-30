using Intranet.DTOs.Requests;
using Intranet.DTOs.Responses;
using Microsoft.AspNetCore.Identity;

namespace Intranet.Security
{
    public interface IJwtService
    {
        public Task<AuthResponse> generateJWT(IdentityUser user);
        public string GenerateRandomToken(int length = 32);
        public Task<AuthResponse> VerifyAndGenerateNewJwtWithRefreshToken(RefreshTokenRequest refreshTokenRequest);

    }
}
