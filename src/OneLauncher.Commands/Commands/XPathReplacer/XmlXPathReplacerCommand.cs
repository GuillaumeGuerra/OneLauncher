using System.Xml.Serialization;
using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.XPathReplacer
{
    [XmlName("XPath")]
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