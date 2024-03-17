using System.Text;

namespace SPM;

internal class Commands
{
    private List<LoginCredentials> _logins = [];
    private byte[] _password = [];
    private string? _defaultFilePath;
    public void Initialize()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string folderPath = Path.Combine(documentsPath, "SPM");
        _defaultFilePath = Path.Combine(folderPath, "Default");
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(_defaultFilePath))
        {
            _password = AskForPassword("Enter password which will be used for encryption. Please, make sure to remember it!: ");
            new AesEncryption().EncryptData(new FileStream(_defaultFilePath, FileMode.Create, FileAccess.Write), _logins, _password);
            return;
        }
        _password = AskForPassword("Enter password to decrypt: ");
        _logins = new AesEncryption().DecryptData<List<LoginCredentials>>(new FileStream(_defaultFilePath, FileMode.Open, FileAccess.Read), _password);
    }

    [Command(CommandName = "add", Usage = "add <service> <login> <password> - Add a password for a service")]
    public void Add(string service, string login, string password)
    {
        string[] parameters = [service, login, password];
        if (parameters.Any(string.IsNullOrEmpty)) throw new ArgumentException();
        _logins.Add(new LoginCredentials(ref parameters));
    }

    [Command(CommandName = "list", Usage = "list - List all stored passwords")]
    public void List()
    {
        foreach (LoginCredentials loginCredentials in _logins)
        {
            Console.WriteLine($"{loginCredentials.Service} {loginCredentials.Login} {loginCredentials.Password}");
        }
    }

    [Command(CommandName = "save", Usage = "save - Save changes to passwords")]
    public void Save()
    {
        new AesEncryption().EncryptData(new FileStream(_defaultFilePath!, FileMode.Create, FileAccess.Write), _logins, _password);
    }

    [Command(CommandName = "remove", Usage = "remove <service> - Remove the password for a service")]
    public void Remove(string service)
    {
        LoginCredentials? loginCredentials = _logins.FirstOrDefault(x => x.Service == service);
        if (string.IsNullOrEmpty(loginCredentials.Value.Login))
        {
            Console.WriteLine($"Entry {service} not found. Run 'list' to view saved entries.");
            return;
        }

        _logins.Remove(loginCredentials.Value);
    }
    public void Exit()
    {
        Save();
        Environment.Exit(0);
    }
    
    [Command(CommandName = "help")]
    private static void DisplayHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  help                 - Display this help message");
        Console.WriteLine("  add <service> <login> <password>   - Add a password for a service");
        Console.WriteLine("  remove <service>     - Remove the password for a service");
        Console.WriteLine("  list                 - List all stored passwords");
        Console.WriteLine("  save                 - Save changes to passwords");
        Console.WriteLine("  exit                 - Save changes and exit the program");
    }

    private static byte[] AskForPassword(string prompt)
    {
        // TODO: Don't show password when typing it.
        string? enteredPassword = null;

        while (string.IsNullOrEmpty(enteredPassword))
        {
            Console.WriteLine(prompt);
            enteredPassword = Console.ReadLine();

            if (string.IsNullOrEmpty(enteredPassword))
            {
                Console.WriteLine("Password cannot be empty. Please try again.");
            }
        }

        return Encoding.UTF8.GetBytes(enteredPassword);
    }
}