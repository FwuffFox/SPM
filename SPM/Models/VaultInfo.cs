namespace SPM.Models;

public struct VaultInfo
{
    public VaultInfo()
    {
    }

    public List<LoginCredentials> Logins { get; set; } = [];
}