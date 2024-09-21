namespace InstaHub.Services.Authentication
{
    public class JwtSettings
    {
        public string JwtSecret { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}
