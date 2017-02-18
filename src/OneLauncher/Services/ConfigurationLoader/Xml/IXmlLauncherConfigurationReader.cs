namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public interface IXmlLauncherConfigurationReader
    {
        XmlLauncherConfiguration LoadFile(string filePath);
    }
}