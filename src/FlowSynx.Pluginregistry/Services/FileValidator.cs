using Microsoft.AspNetCore.Components.Forms;
using System.Security.Cryptography;

namespace FlowSynx.Pluginregistry.Services;

public class FileValidator : IFileValidator
{
    private static readonly string[] AllowedExtensions = { ".fspack" };

    public void Validate(IBrowserFile file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));

        var extension = Path.GetExtension(file.Name)?.ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only '.fspack' files are allowed.");
    }

    public void ValidatePackageContents(string path, out string pluginFile, out string expectedHash)
    {
        expectedHash = File.ReadAllText(Directory.GetFiles(path, "*.sha256").FirstOrDefault()
                                        ?? throw new FileNotFoundException(".sha256 file not found")).Trim();

        pluginFile = Directory.GetFiles(path, "*.plugin").FirstOrDefault()
                     ?? throw new FileNotFoundException(".plugin file not found in the package.");
    }

    public async Task<string> ValidateChecksumAsync(string filePath, string expectedHash, CancellationToken cancellationToken)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);
        var actualHash = Convert.ToHexString(await sha256.ComputeHashAsync(stream, cancellationToken)).ToLowerInvariant();

        if (!expectedHash.Equals(actualHash, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Checksum validation failed.");

        return actualHash;
    }
}