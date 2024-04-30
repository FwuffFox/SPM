using System.Collections.Immutable;
using System.Reflection;
using Spectre.Console;

namespace SPM.Shell;

internal static class Program
{
    private static readonly ImmutableArray<MethodInfo> AllCommands = typeof(Commands)
        .GetMethods()
        .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)
        .ToImmutableArray();
    
    private static void Main(string[] args)
    {
        Console.WriteLine("Welcome to SPM - Simple Password Manager!\nType 'help' to see what we can do.");
        string path;
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

            MethodInfo? foundCommand = AllCommands
                .FirstOrDefault(m =>
                {
                    var commandAttribute = m.GetCustomAttribute<CommandAttribute>();
                    if (commandAttribute is null) return false;
                    return commandAttribute.CommandName == command 
                           || commandAttribute.CommandAliases.Contains(command);
                });
            CallCommand(commands, foundCommand, arguments);
        }
        // ReSharper disable once FunctionNeverReturns
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
            var commandParams = foundCommand.GetParameters();
            if (commandParams.Length == 1 && commandParams[0].ParameterType == typeof(object[]))
                foundCommand.Invoke(commands, [arguments]);
            else foundCommand.Invoke(commands, arguments);
        }
        catch (Exception e) when (e is TargetParameterCountException or ArgumentException)
        {
            Console.WriteLine("Wrong command usage.");
            Console.WriteLine($"Usage: {foundCommand.GetCustomAttribute<CommandAttribute>()!.Usage}");
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }
    }
}