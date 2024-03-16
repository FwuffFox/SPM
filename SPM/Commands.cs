using System.Text;
using SPM;

class Commands
{
    private List<LoginCredentials> _logins = [];
    private byte[] _password = [];
    public void Initialize()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string folderPath = Path.Combine(documentsPath, "SPM");
        string defaultFilePath = Path.Combine(folderPath, "Default");
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            _password = AskForPassword("Enter password which will be used for encryption. Please, make sure to remember it!: ");
            new AesEncryption().EncryptData(new FileStream(defaultFilePath, FileMode.Create, FileAccess.Write), _logins, _password);
            return;
        }
        _password = AskForPassword("Enter password to encrypt: ");
        _logins = new AesEncryption().DecryptData<List<LoginCredentials>>(new FileStream(defaultFilePath, FileMode.Open, FileAccess.Read), _password);
    }

    public void Add(LoginCredentials newCredentials)
    {
        _logins.Add(newCredentials);
    }

    public void List()
    {
        foreach (LoginCredentials loginCredentials in _logins)
        {
            Console.WriteLine($"{loginCredentials.Service} {loginCredentials.Login} {loginCredentials.Password}");
        }
    }
    
    public static byte[] AskForPassword(string prompt)
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