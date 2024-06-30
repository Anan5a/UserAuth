namespace UserAuth.Utility
{
    using BCrypt.Net;

    public static class PasswordManager
    {
        private static readonly int cost = 12;
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, workFactor: cost);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Verify(password, hashedPassword);
        }
    }

}
