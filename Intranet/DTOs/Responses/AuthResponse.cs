namespace Intranet.DTOs.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string? Error { get; set; }

        public AuthResponse(string? token = null, string? refreshToken = null, string? error=null)
        {
            Token = token;
            RefreshToken = refreshToken;
            Error = error;  
        }
    }
}
