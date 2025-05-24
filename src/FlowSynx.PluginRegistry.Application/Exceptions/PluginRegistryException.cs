namespace FlowSynx.PluginRegistry.Application.Exceptions;

public class PluginRegistryException : Exception
{
    public PluginRegistryException(string message)
        : base(message)
    {
    }

    public PluginRegistryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}