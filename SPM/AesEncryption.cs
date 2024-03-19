using System.Security.Cryptography;
using System.Text.Json;

namespace SPM;

public class AesEncryption
{
    /// <summary>
    /// Encrypts data and writes it to passed File.
    /// </summary>
    /// <param name="filePath">File to write to.</param>
    /// <param name="inputData">Data to encrypt.</param>
    /// <param name="password">Password used for encryption.</param>
    /// <returns>true if succeeded; false otherwise.</returns>
    public static bool TryEncryptToFile<T>(string filePath, T inputData, ReadOnlySpan<byte> password)
    {
        try
        {
            EncryptData(new FileStream(filePath, FileMode.Create, FileAccess.Write),
                inputData, password);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    /// <summary>
    /// Decrypts data from a file.
    /// </summary>
    /// <param name="filePath">File to decrypt from.</param>
    /// <param name="outputData">Decrypted data. Null if decryption failed.</param>
    /// <param name="password">Password used for decryption.</param>
    /// <returns>true if succeeded; false otherwise.</returns>
    public static bool TryDecryptFromFile<T>(string filePath, out T? outputData, ReadOnlySpan<byte> password)
    {
        try
        {
            outputData = DecryptData<T>(new FileStream(filePath, FileMode.Open, FileAccess.Read), password);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            outputData = default;
            return false;
        }
    }
    
    /// <summary>
    /// Encrypts data and writes it to passed Stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="inputData">Data to encrypt.</param>
    /// <param name="password">Password used for encryption.</param>
    /// <param name="leaveStreamOpen">True to not close the stream; otherwise, false.</param>
    public static void EncryptData<T>(
        Stream stream,
        T inputData,
        ReadOnlySpan<byte> password,
        bool leaveStreamOpen = false)
    {
        Span<byte> key = stackalloc byte[32];
        Span<byte> iv = stackalloc byte[16];
        HashPasswordToKeyAndIv(password, key, iv);
        
        using ICryptoTransform encryptor = Aes.Create().CreateEncryptor(key.ToArray(), iv.ToArray());
        using CryptoStream cryptoStream = new(stream, encryptor, CryptoStreamMode.Write, leaveStreamOpen);

        JsonSerializer.Serialize(cryptoStream, inputData, JsonSerializerOptions.Default);
    }

    /// <summary>
    /// Decrypts data from a Stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="password">Password used for encryption.</param>
    /// <param name="leaveStreamOpen">True to not close the stream; otherwise, false.</param>
    public static T DecryptData<T>(
        Stream stream,
        ReadOnlySpan<byte> password,
        bool leaveStreamOpen = false)
    {
        Span<byte> key = stackalloc byte[32];
        Span<byte> iv = stackalloc byte[16];
        HashPasswordToKeyAndIv(password, key, iv);
        
        using ICryptoTransform decryptor = Aes.Create().CreateDecryptor(key.ToArray(), iv.ToArray());
        using CryptoStream cryptoStream = new(stream, decryptor, CryptoStreamMode.Read, leaveStreamOpen);
        return JsonSerializer.Deserialize<T>(cryptoStream, JsonSerializerOptions.Default)!;
    }

    private static void HashPasswordToKeyAndIv(
        ReadOnlySpan<byte> password,
        Span<byte> hashedPasswordKey,
        Span<byte> hashedPasswordIv)
    {
        SHA256.HashData(password, hashedPasswordKey);
        MD5.HashData(password, hashedPasswordIv);
    }

}