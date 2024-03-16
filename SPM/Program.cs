using System;
using System.Text;
using System.Text.Json;
using SPM;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to SPM - Simple Password Manager!\nType 'help' to see what we can do.");

        var commands = new Commands();
        commands.Initialize();
        // Main loop for accepting user commands
        while (true)
        {
            Console.Write("SPM > ");
            string input = Console.ReadLine()!.Trim();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            string[] commandParts = input.Split(' ');
            string command = commandParts[0];
            string[] arguments = commandParts.Length > 1 ? commandParts[1..] : [];

            switch (command.ToLower())
            {
                case "help":
                    DisplayHelp();
                    break;
                
                case "add":
                    // TODO: Test implementation. Add actual one.
                    commands.Add(new LoginCredentials($"test{Random.Shared.Next()}", $"{Random.Shared.Next()}", $"{Random.Shared.Next()}"));
                    break;
                
                case "list":
                    commands.List();
                    break;
    
                default:
                    Console.WriteLine("Invalid command. Type 'help' to see available commands.");
                    break;
            }
        }
    }

    static void DisplayHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  help                 - Display this help message");
        Console.WriteLine("  add <service> <password>   - Add a password for a service");
        Console.WriteLine("  update <service> <new_password> - Update the password for a service");
        Console.WriteLine("  remove <service>     - Remove the password for a service");
        Console.WriteLine("  list                 - List all stored passwords");
        Console.WriteLine("  save                 - Save changes to passwords");
        Console.WriteLine("  exit                 - Save changes and exit the program");
    }
}
