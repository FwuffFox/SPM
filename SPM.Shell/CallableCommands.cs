using System.Reflection;
using Spectre.Console;
using SPM.Models;
using SPM.Shell.Extensions;

namespace SPM.Shell;

public partial class Commands
{
    [Command(CommandName = "add", Usage = "add - Add a password for a service")]
    public void Add()
    {
        string service = AnsiConsole.Prompt(SpectreExtensions.CreateTextPrompt<string>("Enter service name:"));
        string login = AnsiConsole.Prompt(SpectreExtensions.CreateTextPrompt<string>("Enter login:"));
        string password = AnsiConsole.Prompt(
            SpectreExtensions.CreatePasswordPrompt("Enter password (or leave empty to generate one):")
                .AllowEmpty()
            );
        if (password.Length != 0)
        {
            _vault.Add(new LoginCredentials(service, login, password));
            AnsiConsole.Write(
                new Table()
                    .AddColumn("Service")
                    .AddColumn("Login")
                    .AddColumn("Password")
                    .AddRow(service, login, password)
                );
            return;
        }
        
        // TODO: Generation
        throw new NotImplementedException();
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