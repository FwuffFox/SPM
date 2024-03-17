namespace SPM;

public struct LoginCredentials(string service, string login, string password)
{
    public LoginCredentials(ref string[] parameters) :
        this(parameters[0], parameters[1], parameters[2]) {}
    
    public string Service { get; set; } = service;
    public string Login { get; set; } = login;
    public string Password { get; set; } = password;
}