using FlowSynx.PluginRegistry.Application.Services;
using System.Security.Cryptography;
using System.Text;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class Gravatar : IGravatar
{
    public string GetGravatarUrl(string email, int size = 100)
    {
        var trimmedLower = email.Trim().ToLowerInvariant();
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(trimmedLower));
        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        return $"https://www.gravatar.com/avatar/{hashString}?s={size}&d=identicon";
    }
}