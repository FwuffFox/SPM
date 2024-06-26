using Spectre.Console;
using SPM.Models;

namespace SPM.Shell.Extensions;

public static class SpectreExtensions
{
    public static TextPrompt<T> CreateTextPrompt<T>(string prompt)
    {
        return new TextPrompt<T>($"[blue]{prompt}[/]");
    }
    
    public static TextPrompt<string> AddPasswordValidation(this TextPrompt<string> obj)
    {
        return obj.Validate(pass =>
        {
            if (pass.Length < 8) return ValidationResult.Error("Password is too short. Use 8 or more characters.");
            if (!(pass.Any(char.IsDigit) || pass.Any(char.IsLetter) || pass.All(ch => !char.IsLetterOrDigit(ch))))
                return ValidationResult.Error(
                    "Password should contain at least one letter, one digit and a special character.");
            return ValidationResult.Success();
        });
    }
    
    public static TextPrompt<string> CreatePasswordPrompt(string prompt, bool validate = false)
    {
        var textPrompt = CreateTextPrompt<string>(prompt)
            .PromptStyle("red")
            .Secret();
        if (validate)
            textPrompt.AddPasswordValidation();

        return textPrompt;
    }

    internal static IEnumerable<T> CreateMultiSelectionPrompt<T>(this IEnumerable<T> choices, string prompt) where T : notnull
    {
        var selectionPrompt = new MultiSelectionPrompt<T>()
            .Title($"[blue]{prompt}[/]")
            .NotRequired()
            .AddChoices(choices)
            .UseConverter(item => item.ToString()!);

        return AnsiConsole.Prompt(selectionPrompt);
    }
    
    internal static T CreateSelectionPrompt<T>(this IEnumerable<T> choices, string prompt) where T : notnull
    {
        var selectionPrompt = new SelectionPrompt<T>()
            .Title($"[blue]{prompt}[/]")
            .AddChoices(choices)
            .UseConverter(item => item.ToString()!);

        return AnsiConsole.Prompt(selectionPrompt);
    }
    
    public static void DisplayError(string error)
    {
        AnsiConsole.MarkupLineInterpolated($"[red]{error}[/]");
    }

    public static void DisplayLoginCredentialsInTable(this IEnumerable<LoginCredentials> loginCredentials)
    {
        Table table = new Table()
            .AddColumn("Service")
            .AddColumn("Login")
            .AddColumn("Password");
        
        foreach (LoginCredentials login in loginCredentials)
        {
            table.AddRow(login.Service.EscapeMarkup(), login.Login.EscapeMarkup(),
                login.Password.EscapeMarkup());
        }
        
        AnsiConsole.Write(table);
    }
    
    public static void DisplayLoginCredentialInTable(this LoginCredentials loginCredentials)
    {
        Table table = new Table()
            .AddColumn("Service")
            .AddColumn("Login")
            .AddColumn("Password");

        table.AddRow(loginCredentials.Service.EscapeMarkup(), loginCredentials.Login.EscapeMarkup(),
            loginCredentials.Password.EscapeMarkup());
        
        AnsiConsole.Write(table);
    }

    public static string AskForPassword()
    {
        string input = AnsiConsole.Prompt(
            CreatePasswordPrompt("Enter password (or leave empty to generate one):")
                .AllowEmpty()
        );
        return !string.IsNullOrWhiteSpace(input) ? input : PasswordGenerator.GenerateSecurePassword();
    }
}