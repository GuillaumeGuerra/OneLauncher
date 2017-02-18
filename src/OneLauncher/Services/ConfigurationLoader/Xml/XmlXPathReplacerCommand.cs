using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlXPathReplacerCommand : XmlLauncherCommand
    {
        [XmlAttribute]
        public string XPath { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        [XmlAttribute]
        public string FilePath { get; set; }

        public override LauncherCommand ToCommand(string resolvedRootPath)
        {
            return new XPathReplacerCommand()
            {
                FilePath = FilePath.Replace(RootToken, resolvedRootPath),
                XPath = XPath,
                Value = Value
            };
        }
    }
}