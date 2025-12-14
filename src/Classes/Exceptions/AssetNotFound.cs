namespace Ethereal.Classes.Exceptions;

internal class AssetNotFoundException : System.Exception
{
    public AssetNotFoundException() { }

    public AssetNotFoundException(string message)
        : base(message) { }

    public AssetNotFoundException(string message, System.Exception innerException)
        : base(message, innerException) { }
}
