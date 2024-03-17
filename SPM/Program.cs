using System.Reflection;

namespace SPM;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Welcome to SPM - Simple Password Manager!\nType 'help' to see what we can do.");

        var commands = new Commands();
        commands.Initialize();
        Console.CancelKeyPress += delegate { commands.Save(); };
        
        MainLoop(commands);
    }

    private static void MainLoop(Commands commands)
    {
        while (true)
        {
            Console.Write("SPM > ");
            string input = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            string[] commandParts = input.Split(' ');
            string command = commandParts[0];
            object[] arguments = commandParts.Length > 1 
                ? commandParts[1..].Select(x => (object) x).ToArray()
                : [];

            MethodInfo? foundCommand = typeof(Commands).GetMethods()
                .FirstOrDefault(m => m.GetCustomAttribute<CommandAttribute>()?.CommandName == command);
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