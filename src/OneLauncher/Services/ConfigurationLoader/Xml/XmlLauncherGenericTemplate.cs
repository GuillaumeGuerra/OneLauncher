using System.Collections.Generic;
using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    [XmlRoot("GenericTemplate")]
    public class XmlLauncherNode
    {
        [XmlAttribute]
        public string Header { get; set; }

        [XmlArray("SubGroups")]
        [XmlArrayItem("Group")]
        public List<XmlLauncherNode> SubGroups { get; set; }

        [XmlArray("Launchers")]
        [XmlArrayItem("Launcher")]
        public List<XmlLauncherLink> Launchers { get; set; }
    }
}