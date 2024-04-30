using System.Security.Cryptography;
using System.Text;

namespace SPM;

public static class PasswordGenerator
{
    private const string Letters = "ABCDEFGHJKLMNOPRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    private const string Numbers = "23456789";
    private const string SpecialCharacters = """!"#$%&'()*+,-./:;<=>?@[\]^_{|}~""";

    public static string GenerateSecurePassword(int length = 8,
        bool useLetters = true,
        bool useNumbers = true,
        bool useSpecialCharacters = true)
    {
        var allowedCharacters = new StringBuilder();
        if (useLetters) allowedCharacters.Append(Letters);
        if (useNumbers) allowedCharacters.Append(Numbers);
        if (useSpecialCharacters) allowedCharacters.Append(SpecialCharacters);

        var password = new StringBuilder();
        string passwordString;
        do
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(length);

            foreach (byte randomByte in randomBytes)
            {
                password.Append(allowedCharacters[randomByte % allowedCharacters.Length]);
            }

            passwordString = password.ToString();
        } while (!CheckPassword(passwordString));
        
        return passwordString;
    }

    private static bool CheckPassword(
        string password,
        bool useLetters = true,
        bool useNumbers = true,
        bool useSpecialCharacters = true)
    {
        if (useLetters && !password.Any(c => Letters.Contains(c))) return false;
        if (useNumbers && !password.Any(c => Numbers.Contains(c))) return false;
        if (useSpecialCharacters && !password.Any(c => SpecialCharacters.Contains(c))) return false;
        return true;
    }
}