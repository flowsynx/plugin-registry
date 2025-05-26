using System.Security.Cryptography;
using System.Text;

namespace FlowSynx.PluginRegistry.Infrastructure.Helpers;

public static class ApiKeyHelper
{
    private const string DefaultPrefix = "fspr_";

    public static string Generate(string? prefix = null, int length = 40)
    {
        if (length < 16)
            throw new ArgumentException("Key length should be at least 16 bytes for security.");

        byte[] bytes = RandomNumberGenerator.GetBytes(length);
        string base64 = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        // Trim to requested length, in case base64 exceeds it
        var key = base64.Length > length ? base64[..length] : base64;

        return $"{(prefix ?? DefaultPrefix)}{key}";
    }

    public static string Hash(string rawKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(rawKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}