using System.Security.Cryptography;
using SPM.Models;

namespace SPM;

/// <summary>
/// Represents a secure vault for storing login credentials.
/// </summary>
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
    /// Opens an existing vault.
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

    /// <summary>
    /// Saves the current state of the vault to a file.
    /// </summary>
    /// <param name="vaultPath">Path to the vault file.</param>
    /// <param name="passwordUtf8Bytes">Password used for encryption.</param>
    public void SaveVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes)
    {
        AesEncryption.EncryptToFile(vaultPath, vaultInfo, passwordUtf8Bytes);
    }

    /// <summary>
    /// Retrieves login credentials stored in the vault.
    /// </summary>
    /// <returns>A read-only list of login credentials.</returns>
    public IEnumerable<LoginCredentials> GetLoginCredentials()
    {
        return vaultInfo.Logins;
    }
    
    /// <summary>
    /// Retrieves login credentials stored in the vault.
    /// </summary>
    /// <returns>A read-only list of login credentials.</returns>
    public IEnumerable<LoginCredentials> GetLoginCredentials(string contains)
    {
        return vaultInfo.Logins.Where(login => login.Service.Contains(contains, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Adds a loginCredentials if it doesn't exist already
    /// </summary>
    /// <param name="loginCredentials">The login credentials to add.</param>
    public void Add(LoginCredentials loginCredentials)
    {
        if (vaultInfo.Logins.Contains(loginCredentials)) return;
        vaultInfo.Logins.Add(loginCredentials);
    }

    /// <summary>
    /// Removes a loginCredentials if it exists.
    /// </summary>
    /// <param name="loginCredentials">Login to remove.</param>
    /// <returns> true if item is successfully removed; otherwise, false.
    /// This method also returns false if item was not found</returns>
    public bool Remove(LoginCredentials loginCredentials)
    {
        return vaultInfo.Logins.Remove(loginCredentials);
    }

    /// <summary>
    /// Removes a list of login credentials from the vault.
    /// </summary>
    /// <param name="loginCredentials">The list of login credentials to remove.</param>
    /// <returns>Always returns true.</returns>
    public bool Remove(IEnumerable<LoginCredentials> loginCredentials)
    {
        foreach (LoginCredentials login in loginCredentials)
        {
            vaultInfo.Logins.Remove(login);
        }

        return true;
    }
    
    public void Update(LoginCredentials old, LoginCredentials @new)
    {
        int index = vaultInfo.Logins.IndexOf(old);
        vaultInfo.Logins.Remove(old);
        vaultInfo.Logins.Insert(index, @new);
    }
}