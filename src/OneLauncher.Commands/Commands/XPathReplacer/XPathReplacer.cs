using System.IO;
using System.Xml;
using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.XPathReplacer
{
    public class XPathReplacer : BaseCommandLauncher<XPathReplacerCommand>
    {
        protected override void DoExecute(XPathReplacerCommand command)
        {
            if(!File.Exists(command.FilePath))
                throw new FileNotFoundException($"Unable to find file [{command.FilePath}] for xpath replacement");

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(command.FilePath);

            var xmlNodeList = xmlDoc.SelectNodes(command.XPath);
            foreach (var node in xmlNodeList)
            {
                var attribute = node as XmlAttribute;
                if (attribute != null)
                    attribute.Value = command.Value;
            }

            xmlDoc.Save(command.FilePath);
        }
    }
}