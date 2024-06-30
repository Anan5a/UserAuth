namespace UserAuth.Utility
{
    public static class ConfigHelper
    {
        public static string GetAPIKey(IConfiguration configuration) => configuration["ApiKey"];
        public static string GetApiUrl(IConfiguration configuration) => configuration["ApiUrl"];
    }
}
