using SPM;
using Xunit;
using Xunit.Abstractions;

namespace SPM.Tests;

public class EncryptionTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EncryptionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        var password = "TestPassword"u8.ToArray().AsSpan();
        var loginCredentials = new LoginCredentials("github.com", "TestLogin", "TestPassword");
        
        var enc = new AesEncryption();
        using var encryptedDataStream = new MemoryStream();
        enc.EncryptData(encryptedDataStream, loginCredentials, password, true);
        encryptedDataStream.Position = 0;
        var decryptedData = enc.DecryptData<LoginCredentials>(encryptedDataStream, password);
        Assert.Equal(loginCredentials, decryptedData);
    }
}