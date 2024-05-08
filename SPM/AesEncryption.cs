using System.Security.Cryptography;
using System.Text.Json;

namespace SPM;

public static class AesEncryption
{
    /// <summary>
    /// Encrypts data and writes it to passed File.
    /// </summary>
    /// <param name="filePath">File to write to.</param>
    /// <param name="inputData">Data to encrypt.</param>
    /// <param name="password">Password used for encryption.</param>
    public static void EncryptToFile<T>(string filePath, T inputData, ReadOnlySpan<byte> password)
    {
        EncryptData(new FileStream(filePath, FileMode.Create, FileAccess.Write), inputData, password);
    }

    /// <summary>
    /// Decrypts data from a file.
    /// </summary>
    /// <param name="filePath">File to decrypt from.</param>
    /// <param name="password">Password used for decryption.</param>
    /// <exception cref="FileNotFoundException">File can't be found.</exception>
    public static T DecryptFromFile<T>(string filePath, ReadOnlySpan<byte> password)
    {
        return DecryptData<T>(new FileStream(filePath, FileMode.Open, FileAccess.Read), password);
    }
    
    /// <summary>
    /// Encrypts data and writes it to passed Stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="inputData">Data to encrypt.</param>
    /// <param name="password">Password used for encryption.</param>
    /// <param name="leaveStreamOpen">True to not close the stream; otherwise, false.</param>
    /// /// <exception cref="CryptographicException">Failed to encrypt.</exception>
    public static void EncryptData<T>(
        Stream stream,
        T inputData,
        ReadOnlySpan<byte> password,
        bool leaveStreamOpen = false)
    {
        using ICryptoTransform encryptor = CreateEncryptor(password);
        using CryptoStream cryptoStream = new(stream, encryptor, CryptoStreamMode.Write, leaveStreamOpen);

        JsonSerializer.Serialize(cryptoStream, inputData, JsonSerializerOptions.Default);
    }

    /// <summary>
    /// Decrypts data from a Stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <param name="password">Password used for encryption.</param>
    /// <param name="leaveStreamOpen">True to not close the stream; otherwise, false.</param>
    /// <exception cref="CryptographicException">Failed to decrypt. Either password is incorrect or file is corrupted.</exception>
    public static T DecryptData<T>(
        Stream stream,
        ReadOnlySpan<byte> password,
        bool leaveStreamOpen = false)
    {
        using ICryptoTransform decryptor = CreateDecryptor(password);
        using CryptoStream cryptoStream = new(stream, decryptor, CryptoStreamMode.Read, leaveStreamOpen);
        return JsonSerializer.Deserialize<T>(cryptoStream, JsonSerializerOptions.Default)!;
    }

    private static ICryptoTransform CreateDecryptor(ReadOnlySpan<byte> password)
    {
        Span<byte> key = stackalloc byte[32];
        Span<byte> iv = stackalloc byte[16];
        Span<byte> salt = stackalloc byte[16];
        MD5.HashData(password, salt);
        HashPasswordToKeyAndIv(password, key, iv, salt);

        return Aes.Create().CreateDecryptor(key.ToArray(), iv.ToArray());
    }
    
    private static ICryptoTransform CreateEncryptor(ReadOnlySpan<byte> password)
    {
        Span<byte> key = stackalloc byte[32];
        Span<byte> iv = stackalloc byte[16];
        Span<byte> salt = stackalloc byte[16];
        MD5.HashData(password, salt);
        HashPasswordToKeyAndIv(password, key, iv, salt);

        return Aes.Create().CreateEncryptor(key.ToArray(), iv.ToArray());
    }
    
    private static void HashPasswordToKeyAndIv(
        ReadOnlySpan<byte> password,
        Span<byte> hashedPasswordKey,
        Span<byte> hashedPasswordIv,
        ReadOnlySpan<byte> salt)
    {
        Span<byte> kdf = stackalloc byte[48];
        Rfc2898DeriveBytes.Pbkdf2(password, salt, kdf, 10000, HashAlgorithmName.SHA512);
        kdf[..32].CopyTo(hashedPasswordKey);
        kdf[32..].CopyTo(hashedPasswordIv);
    }
}