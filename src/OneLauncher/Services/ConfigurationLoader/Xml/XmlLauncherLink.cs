using System.Collections.Generic;
using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlLauncherLink
    {
        [XmlAttribute]
        public string Header { get; set; }

        [XmlArray("Commands")]
        [XmlArrayItem("Execute", typeof(XmlExecuteCommand))]
        [XmlArrayItem("XPath", typeof(XmlXPathReplacerCommand))]
        [XmlArrayItem("File", typeof(XmlFileCopierCommand))]
        public List<XmlLauncherCommand> Commands { get; set; }
    }
}