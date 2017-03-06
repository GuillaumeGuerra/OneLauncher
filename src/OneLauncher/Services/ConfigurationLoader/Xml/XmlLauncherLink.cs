using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using OneLauncher.Core.Commands;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlLauncherLink
    {
        [XmlAttribute]
        public string Header { get; set; }

        [XmlArray("Commands")]
        public List<XmlLauncherCommand> Commands { get; set; }
    }
}