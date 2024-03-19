using System.Reflection;

namespace SPM.Shell;

internal static class Program
{
    private static void Main(string[] args)
    {
        System.Console.WriteLine("Welcome to SPM - Simple Password Manager!\nType 'help' to see what we can do.");
        string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SPM",
            "Default.enc");
        var commands = new Commands(defaultPath);
        System.Console.CancelKeyPress += delegate { commands.Save(); };
        
        MainLoop(commands);
    }

    private static void MainLoop(Commands commands)
    {
        while (true)
        {
            System.Console.Write("SPM > ");
            string input = System.Console.ReadLine()!.Trim();

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
            System.Console.WriteLine("Command is not found. Use 'help' to get information about commands.");
            return;
        }

        try
        {
            foundCommand.Invoke(commands, arguments);
        }
        catch (Exception _) when (_ is TargetParameterCountException or ArgumentException)
        {
            System.Console.WriteLine("Wrong command usage.");
            System.Console.WriteLine($"Usage: {foundCommand.GetCustomAttribute<CommandAttribute>()!.Usage}");
        }
    }
}