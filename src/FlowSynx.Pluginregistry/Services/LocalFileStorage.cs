namespace FlowSynx.Pluginregistry.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly string _basePath;

    public LocalFileStorage(IWebHostEnvironment env, IConfiguration configuration)
    {
        var relativePath = configuration["Storage:Local:RelativePath"] ?? "App_Data";
        _basePath = Path.Combine(env.ContentRootPath, relativePath);
        Directory.CreateDirectory(_basePath);
    }

    public async Task SaveFileAsync(string path, byte[] content)
    {
        var fullPath = Path.Combine(_basePath, path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await File.WriteAllBytesAsync(fullPath, content);
    }

    public async Task<byte[]> ReadFileAsync(string path)
    {
        var fullPath = Path.Combine(_basePath, path);
        var content = await File.ReadAllBytesAsync(fullPath);
        return content;
    }

    public Task DeleteFileAsync(string path)
    {
        var fullPath = Path.Combine(_basePath, path);
        File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string path)
    {
        var fullPath = Path.Combine(_basePath, path);
        return Task.FromResult(File.Exists(fullPath));
    }
}