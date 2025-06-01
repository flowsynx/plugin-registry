namespace FlowSynx.Pluginregistry.Services;

public interface IFileStorage
{
    Task SaveFileAsync(string path, byte[] content);
    Task<byte[]> ReadFileAsync(string path);
    Task DeleteFileAsync(string path);
    Task<bool> FileExistsAsync(string path);
}