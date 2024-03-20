using System.Security.Cryptography;
using System.Text;
using Spectre.Console;
using SPM.Shell.Extensions;

namespace SPM.Shell;

public partial class Commands
{
    private byte[] _password = [];
    private readonly string _pathToVault;
    private Vault _vault = null!;
    
    public Commands(string pathToVault)
    {
        _pathToVault = pathToVault;
        if (!File.Exists(pathToVault))
        {
            CreateNewVault(pathToVault);
            return;
        }
        OpenExistingVault(pathToVault);
    }

    private void OpenExistingVault(string pathToVault)
    {
        while (true)
        {
            try
            {
                Console.WriteLine($"Trying to open the vault at {pathToVault}");
                _password = AnsiConsole.Prompt(
                    SpectreExtensions.CreatePasswordPrompt("Enter password to decrypt the vault:")
                ).GetUtf8Bytes();
                _vault = Vault.TryOpenVault(pathToVault, _password);
                break;
            }
            catch (CryptographicException e)
            {
                AnsiConsole.WriteException(e);
            }
        }
    }

    private void CreateNewVault(string pathToVault)
    {
        while (true)
        {
            try
            {
                Console.WriteLine($"Trying to create the vault at {pathToVault}");
                
                _password = AnsiConsole.Prompt(
                    SpectreExtensions.CreatePasswordPrompt("Enter password to create a new Vault:", true)
                ).GetUtf8Bytes();
                
                _vault = Vault.CreateVault(pathToVault, _password);
                break;
            }
            catch (CryptographicException e)
            {
                AnsiConsole.WriteException(e);
            }
        }
    }
}