using System.Security.Cryptography;
using System.Text.Json;

namespace SPM;

public class AesEncryption
{
    /// <summary>
    /// Encrypts data and writes it to passed Stream.
    /// </summary>
    /// <param name="stream">Stream to write to.</param>
    /// <param name="inputData">Data to encrypt and write to the Stream.</param>
    /// <param name="password">True to not close the stream; otherwise, false.</param>
    public void EncryptData<T>(
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
    public T DecryptData<T>(
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

    private void HashPasswordToKeyAndIv(
        ReadOnlySpan<byte> password,
        Span<byte> hashedPasswordKey,
        Span<byte> hashedPasswordIv)
    {
        SHA256.TryHashData(password, hashedPasswordKey, out _);
        MD5.TryHashData(password, hashedPasswordIv, out _);
    }

}