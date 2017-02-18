using System.Collections.Generic;
using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    [XmlRoot("Configuration")]
    public class XmlLauncherConfiguration
    {
        [XmlArray("RootDirectories")]
        [XmlArrayItem("RootDirectory")]
        public List<XmlLauncherRootDirectory> RootDirectories { get; set; }
        
        public XmlLauncherNode GenericTemplate { get; set; }
    }
}