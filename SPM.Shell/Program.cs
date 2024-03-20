using System.Reflection;
using System.Text.Json;
using Spectre.Console;

namespace SPM.Shell;

internal static class Program
{
    private static void Main(string[] args)
    {
        string path;
        Console.WriteLine("Welcome to SPM - Simple Password Manager!\nType 'help' to see what we can do.");
        if (args.Length != 0)
        {
            path = Path.GetFullPath(args[0]);
        }
        else path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SPM",
            "Default.enc");
        var commands = new Commands(path);
        Console.CancelKeyPress += delegate { commands.Save(); };
        
        MainLoop(commands, path);
    }

    private static void MainLoop(Commands commands, string path)
    {
        while (true)
        {
            AnsiConsole.Markup("[bold green]SPM [/]");
            AnsiConsole.Write(new TextPath(path)
                .RootColor(Color.Green)
                .SeparatorColor(Color.Green)
                .LeafColor(Color.Aqua)
                .StemColor(Color.Green)
            );
            AnsiConsole.Markup("[bold green]> [/]");
            string input = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            string[] commandParts = input.Split(' ');
            string command = commandParts[0];
            object[] arguments = commandParts.Length > 1 
                ? commandParts[1..].Select(x => (object) x).ToArray()
                : [];

            MethodInfo? foundCommand = typeof(Commands).GetMethods()
                .FirstOrDefault(m =>
                {
                    var commandAttribute = m.GetCustomAttribute<CommandAttribute>();
                    if (commandAttribute is null) return false;
                    return commandAttribute.CommandName == command 
                           || commandAttribute.CommandAliases.Contains(command);
                });
            CallCommand(commands, foundCommand, arguments);
        }
    }

    private static void CallCommand(Commands commands, MethodInfo? foundCommand, object[] arguments)
    {
        if (foundCommand is null)
        {
            Console.WriteLine("Command is not found. Use 'help' to get information about commands.");
            return;
        }

        try
        {
            foundCommand.Invoke(commands, arguments);
        }
        catch (Exception _) when (_ is TargetParameterCountException or ArgumentException)
        {
            Console.WriteLine("Wrong command usage.");
            Console.WriteLine($"Usage: {foundCommand.GetCustomAttribute<CommandAttribute>()!.Usage}");
        }
    }
}