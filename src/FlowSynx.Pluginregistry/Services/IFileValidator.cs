using Microsoft.AspNetCore.Components.Forms;

namespace FlowSynx.Pluginregistry.Services;

public interface IFileValidator
{
    void Validate(IBrowserFile file);
    void ValidatePackageContents(string path, out string pluginFile, out string expectedHash);
    Task<string> ValidateChecksumAsync(string filePath, string expectedHash, CancellationToken cancellationToken);
}