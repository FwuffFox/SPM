using System.Collections;
using System.Collections.ObjectModel;
using SPM.Models;

namespace SPM;

public class Vault(VaultInfo vaultInfo)
{
    public static bool TryCreateVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes, out Vault newVault)
    {
        var newVaultInfo = new VaultInfo();
        newVault = new Vault(newVaultInfo);
        
        return File.Exists(vaultPath) &&
               AesEncryption.TryEncryptToFile(vaultPath, newVaultInfo, passwordUtf8Bytes);
    }
    
    public static bool TryOpenVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes, out Vault decryptedVault)
    {
        decryptedVault = new Vault(default);
        if (!AesEncryption.TryDecryptFromFile(vaultPath,
                out VaultInfo decryptedVaultInfo, passwordUtf8Bytes)) return false;
        decryptedVault = new Vault(decryptedVaultInfo);
        return true;
    }

    public bool TrySaveVault(string vaultPath, ReadOnlySpan<byte> passwordUtf8Bytes)
    {
        return AesEncryption.TryEncryptToFile(vaultPath, vaultInfo, passwordUtf8Bytes);
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