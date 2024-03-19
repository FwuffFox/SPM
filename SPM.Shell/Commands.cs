using System.Security.Cryptography;
using System.Text;
using Spectre.Console;

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
                _password = AskForPassword("Enter password to decrypt the vault: ");
                _vault = Vault.TryOpenVault(pathToVault, _password);
                break;
            }
            catch (CryptographicException e)
            {
                Utilities.WriteErrorLine(e.Message);
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
                _password = AskForPassword("Enter password to create a new Vault: ", true);
                _vault = Vault.CreateVault(pathToVault, _password);
                break;
            }
            catch (CryptographicException e)
            {
                Utilities.WriteErrorLine(e.Message);
            }
        }
    }

    private static byte[] AskForPassword(string prompt, bool validate = false)
    {
        var textPrompt = new TextPrompt<string>(prompt)
            .PromptStyle("red")
            .Secret();
        if (validate)
            textPrompt.Validate(pass =>
            {
                if (pass.Length < 8) return ValidationResult.Error("Password is too short. Use 8 or more characters.");
                if (!(pass.Any(char.IsDigit) || pass.Any(char.IsLetter) || pass.All(ch => !char.IsLetterOrDigit(ch))))
                    return ValidationResult.Error(
                        "Password should contain at least one letter, one digit and a special character.");
                return ValidationResult.Success();
            });
        
        return Encoding.UTF8.GetBytes(AnsiConsole.Prompt(textPrompt));
    }
}