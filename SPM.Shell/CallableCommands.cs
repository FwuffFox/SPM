using System.Reflection;
using Spectre.Console;
using SPM.Models;

namespace SPM.Shell;

public partial class Commands
{
    [Command(CommandName = "add", Usage = "add <service> <login> <password> - Add a password for a service")]
    public void Add(string service, string login, string password)
    {
        string[] parameters = [service, login, password];
        if (parameters.Any(string.IsNullOrEmpty)) throw new ArgumentException();
        _vault.Add(new LoginCredentials(ref parameters));
    }

    [Command(CommandName = "list", CommandAliases = ["ls"],
        Usage = "list - List all stored passwords")]
    public void List()
    {
        Table table = new Table()
            .AddColumn("Service")
            .AddColumn("Login")
            .AddColumn("Password");
        
        foreach (LoginCredentials loginCredentials in _vault.GetAllLoginCredentials())
        {
            table.AddRow(loginCredentials.Service, loginCredentials.Login, loginCredentials.Password);
        }
        
        AnsiConsole.Write(table);
    }

    [Command(CommandName = "save", Usage = "save - Save changes to passwords")]
    public void Save()
    {
        _vault.SaveVault(_pathToVault, _password);
    }

    [Command(CommandName = "remove", CommandAliases = ["rm"],
        Usage = "remove <service> - Remove the password for a service")]
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
    
    [Command(CommandName = "help", Usage = "Display this help message")]
    public void DisplayHelp()
    {
        IEnumerable<CommandAttribute> attributes = from m in typeof(Commands).GetMethods()
            let attribute = m.GetCustomAttribute<CommandAttribute>()
            where attribute != null
            select attribute;
        
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        foreach (CommandAttribute commandAttribute in attributes)
        {
            Console.WriteLine($"  {commandAttribute.CommandName,-20} - {commandAttribute.Usage}");
            if (commandAttribute.CommandAliases.Length != 0)
            {
                Console.WriteLine($"     Aliases: {string.Join(", ", commandAttribute.CommandAliases)}");
            }
        }
    }
}