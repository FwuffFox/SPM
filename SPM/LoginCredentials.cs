namespace SPM;

public struct LoginCredentials(string service, string login, string password)
{
    public string Service { get; set; } = service;
    public string Login { get; set; } = login;
    public string Password { get; set; } = password;
}