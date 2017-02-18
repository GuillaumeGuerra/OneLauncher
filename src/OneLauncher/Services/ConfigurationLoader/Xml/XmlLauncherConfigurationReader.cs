using System;
using System.IO;
using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlLauncherConfigurationReader : IXmlLauncherConfigurationReader
    {
        public XmlLauncherConfiguration LoadFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlLauncherConfiguration));

            if (!File.Exists(filePath))
                throw new InvalidOperationException(string.Format("Unknow file path [{0}]", filePath));

            var text = File.ReadAllText(filePath);
            using (var reader = new StringReader(text))
            {
                return (XmlLauncherConfiguration)serializer.Deserialize(reader);
            }
        }
    }
}