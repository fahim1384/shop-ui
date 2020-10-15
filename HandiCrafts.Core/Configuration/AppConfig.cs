namespace HandiCrafts.Core.Configuration
{
    public class AppConfig
    {
        public string JwtSecretKey { get; set; }
        public int CacheLifeTimeMinutes { get; set; }
        public bool BypassRecaptcha { get; set; } = false;
        public string Currency { get; set; } = "ریال";
    }
}
