namespace Intranet.Security
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Linked to the AspNet Identity User Id
        public string Token { get; set; }
        public string JwtId { get; set; } // Map the token with jwtId
        public bool IsUsed { get; set; } // if its used we dont want generate a new Jwt token with the same refresh token
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
