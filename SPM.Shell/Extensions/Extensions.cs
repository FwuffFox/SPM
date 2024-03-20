using System.Text;

namespace SPM.Shell.Extensions;

public static class Extensions
{
    public static byte[] GetUtf8Bytes(this string data) => Encoding.UTF8.GetBytes(data);
}