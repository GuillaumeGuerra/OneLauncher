using System.Collections.Generic;
using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    [XmlRoot("Configuration")]
    public class XmlLauncherConfiguration
    {
        [XmlAttribute("RepoType")]
        public string RepoType { get; set; }
        
        public XmlLauncherNode GenericTemplate { get; set; }
    }
}