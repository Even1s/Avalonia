namespace CafeApp.Helpers;

public static class PasswordCrypt
{
    public static string CryptPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool IsValid(string? password, string? hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;
        
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}