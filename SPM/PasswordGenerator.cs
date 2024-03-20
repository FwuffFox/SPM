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
        StringBuilder password = new();
        var allowedCharacters = "";

        if (useLetters)
            allowedCharacters += Letters;
        if (useNumbers)
            allowedCharacters += Numbers;
        if (useSpecialCharacters)
            allowedCharacters += SpecialCharacters;
        
        byte[] randomBytes = RandomNumberGenerator.GetBytes(length);
        
        for (var i = 0; i < length; i++) 
        { 
            int index = randomBytes[i] % allowedCharacters.Length;
            password.Append(allowedCharacters[index]);
        }

        return password.ToString();
    }
}