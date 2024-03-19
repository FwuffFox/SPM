using System.Security.Cryptography;
using SPM.Models;

namespace SPM;

public class Vault(VaultInfo vaultInfo)
{
    /// <summary>
    /// Creates a new vault.
    /// </summary>
    /// <param name="vaultPath">Path to a new vault.</param>
    /// <param name="passwordUtf8Bytes">Password used for encryption.</param>
    /// <returns>An opened vault.</returns>
    /// <exception cref="CryptographicException">Failed to encrypt vault.</exception>
    public static Vault CreateVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes)
    {
        try
        {
            var newVaultInfo = new VaultInfo();
            AesEncryption.EncryptToFile(vaultPath, newVaultInfo, passwordUtf8Bytes);
            return new Vault(newVaultInfo);
        }
        catch (CryptographicException e)
        {
            throw new CryptographicException("Failed to encrypt the vault.");
        }
    }
    
    /// <summary>
    /// Opens a vault.
    /// </summary>
    /// <param name="vaultPath">File to decrypt from.</param>
    /// <param name="passwordUtf8Bytes">Password used for encryption.</param>
    /// <returns>An opened vault.</returns>
    /// <exception cref="FileNotFoundException">File not found.</exception>
    /// <exception cref="CryptographicException">Failed to decrypt vault.</exception>
    public static Vault TryOpenVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes)
    {
        try
        {
            return new Vault(AesEncryption.DecryptFromFile<VaultInfo>(vaultPath, passwordUtf8Bytes));
        }
        catch (FileNotFoundException e)
        {
            throw new FileNotFoundException($"File {vaultPath} not found.", e);
        }
        catch (CryptographicException e)
        {
            throw new CryptographicException("Failed to decrypt vault.", e);
        }
        catch (Exception e)
        {
            throw new Exception("Unknown error at opening vault.", e);
        }
    }

    public void SaveVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes)
    {
        AesEncryption.EncryptToFile(vaultPath, vaultInfo, passwordUtf8Bytes);
    }

    public IEnumerable<LoginCredentials> GetAllLoginCredentials()
    {
        return vaultInfo.Logins.AsReadOnly();
    }
    
    public void Add(LoginCredentials loginCredentials)
    {
        vaultInfo.Logins.Add(loginCredentials);
    }
}