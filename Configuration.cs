namespace Blog
{
    public static class Configuration
    {
        public static string JwtKey { get; set; }
        public static string ApiKeyName { get; set; }
        public static string ApiKeyValue { get; set; }
        public static SmtpConfiguration Smtp = new();
        public static string DefaultConnection { get; set; }
    }

   public class SmtpConfiguration
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
