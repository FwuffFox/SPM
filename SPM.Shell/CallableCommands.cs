using System.Reflection;
using Spectre.Console;
using SPM.Models;
using SPM.Shell.Extensions;

namespace SPM.Shell;

public partial class Commands
{
    [Command(CommandName = "add", Usage = "add - Add a password for a service. Interactive.")]
    public void Add()
    {
        string service = AnsiConsole.Prompt(SpectreExtensions.CreateTextPrompt<string>("Enter service name:"));
        string login = AnsiConsole.Prompt(SpectreExtensions.CreateTextPrompt<string>("Enter login:"));
        string password = AnsiConsole.Prompt(
            SpectreExtensions.CreatePasswordPrompt("Enter password (or leave empty to generate one):")
                .AllowEmpty()
            );
        if (string.IsNullOrWhiteSpace(password))
        {
            password = PasswordGenerator.GenerateSecurePassword();
        }
        _vault.Add(new LoginCredentials(service, login, password));
        AnsiConsole.Write(
            new Table()
                .AddColumn("Service")
                .AddColumn("Login")
                .AddColumn("Password")
                .AddRow(service.EscapeMarkup(), login.EscapeMarkup(), password.EscapeMarkup())
        );
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
            table.AddRow(loginCredentials.Service.EscapeMarkup(), loginCredentials.Login.EscapeMarkup(),
                loginCredentials.Password.EscapeMarkup());
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
        var logins = _vault.GetAllLoginCredentials()
            .Where(login => login.Service.StartsWith(service)).ToArray();

        switch (logins.Length)
        {
            case 0:
                SpectreExtensions.DisplayError($"Service '{service}' not found.");
                return;
            
            case 1:
                if (AnsiConsole.Confirm($"Are you sure you want to remove the login for '{service}'?"))
                {
                    _vault.Remove(logins.First());
                }
                return;
            
            default:
                var selectedLogins = logins.CreateSelectionPrompt("Select logins to remove:").ToArray();
                if (selectedLogins.Length == 0)
                {
                    SpectreExtensions.DisplayError("No logins selected.");
                    return;
                }
                if (AnsiConsole.Confirm($"[blue]Are you sure you want to remove {selectedLogins.Length} selected logins for '{service.EscapeMarkup()}'?[/]"))
                {
                    _vault.Remove(selectedLogins);
                }
                return;
        }
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