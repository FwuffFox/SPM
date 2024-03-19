namespace SPM.Shell;

public static class Utilities
{
    public static void WriteLineColorized(string text, ConsoleColor color)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = oldColor;
    }
    
    public static void WriteColorized(string text, ConsoleColor color)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = oldColor;
    }

    public static void WriteErrorLine(string text) => WriteLineColorized(text, ConsoleColor.Red);
}