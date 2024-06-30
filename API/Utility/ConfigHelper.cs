namespace API.Utility
{
    public static class ConfigHelper
    {
        public static string GetAPIKey(IConfiguration configuration) => configuration["ApiKey"];
        public static string GetConnectionString(IConfiguration configuration) => configuration.GetConnectionString("DefaultConnection");
    }
}
