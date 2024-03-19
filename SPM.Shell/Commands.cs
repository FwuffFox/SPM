using System.Text;
using SPM.Models;

namespace SPM.Shell;

public class Commands
{
    private readonly byte[] _password;
    private string _pathToVault;
    private readonly Vault _vault;
    
    public Commands(string pathToVault)
    {
        _pathToVault = pathToVault;
        if (!File.Exists(pathToVault))
        {
            Console.WriteLine($"Trying to create the vault at {pathToVault}");
            _password = AskForPassword("Enter password to encrypt the vault: ");
            Vault.TryCreateVault(pathToVault, _password, out _vault);
            return;
        }
        Console.WriteLine($"Trying to open the vault at {pathToVault}");
        _password = AskForPassword("Enter password to decrypt the vault: ");
        Vault.TryOpenVault(pathToVault, _password, out _vault);
    }

    [Command(CommandName = "add", Usage = "add <service> <login> <password> - Add a password for a service")]
    public void Add(string service, string login, string password)
    {
        string[] parameters = [service, login, password];
        if (parameters.Any(string.IsNullOrEmpty)) throw new ArgumentException();
        _vault.Add(new LoginCredentials(ref parameters));
    }

    [Command(CommandName = "list", Usage = "list - List all stored passwords")]
    public void List()
    {
        foreach (LoginCredentials loginCredentials in _vault.GetAllLoginCredentials())
        {
            Console.WriteLine($"{loginCredentials.Service} {loginCredentials.Login} {loginCredentials.Password}");
        }
    }

    [Command(CommandName = "save", Usage = "save - Save changes to passwords")]
    public void Save()
    {
        _vault.TrySaveVault(_pathToVault, _password);
    }

    [Command(CommandName = "remove", Usage = "remove <service> - Remove the password for a service")]
    public void Remove(string service)
    {
        throw new NotImplementedException();
    }
    
    [Command(CommandName = "exit", Usage = "exit - Save changes and exit the program")]
    public void Exit()
    {
        Save();
        Environment.Exit(0);
    }
    
    [Command(CommandName = "help")]
    private static void DisplayHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  help                 - Display this help message");
        Console.WriteLine("  add <service> <login> <password>   - Add a password for a service");
        Console.WriteLine("  remove <service>     - Remove the password for a service");
        Console.WriteLine("  list                 - List all stored passwords");
        Console.WriteLine("  save                 - Save changes to passwords");
        Console.WriteLine("  exit                 - Save changes and exit the program");
    }

    private static byte[] AskForPassword(string prompt)
    {
        // TODO: Don't show password when typing it.
        string? enteredPassword = null;

        while (string.IsNullOrEmpty(enteredPassword))
        {
            Console.WriteLine(prompt);
            enteredPassword = Console.ReadLine();

            if (string.IsNullOrEmpty(enteredPassword))
            {
                Console.WriteLine("Password cannot be empty. Please try again.");
            }
        }

        return Encoding.UTF8.GetBytes(enteredPassword);
    }
}