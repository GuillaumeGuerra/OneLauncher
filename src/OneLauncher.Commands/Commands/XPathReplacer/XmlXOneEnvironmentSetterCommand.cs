using System.Xml.Serialization;
using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.XPathReplacer
{
    [XmlName("SetXOneEnvironment")]
    public class XmlXOneEnvironmentSetterCommand : XmlLauncherCommand
    {
        [XmlAttribute]
        public string XOneEnvironment { get; set; }

        [XmlAttribute]
        public string FilePath { get; set; }

        public override LauncherCommand ToCommand(string resolvedRootPath)
        {
            return new XPathReplacerCommand()
            {
                FilePath = FilePath.Replace(RootToken, resolvedRootPath),
                XPath = @"configuration/appSettings/add[@key=""usedEnvironment""]/@value",
                Value = XOneEnvironment
            };
        }
    }
}