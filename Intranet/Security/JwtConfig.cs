using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Intranet.Security
{
    public class JwtConfig
    {
        public string? Key { get; set; }
    }
}
