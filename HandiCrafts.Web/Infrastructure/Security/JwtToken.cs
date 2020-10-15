namespace HandiCrafts.Web.Infrastructure.Security
{
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public int ExpiredIn { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
