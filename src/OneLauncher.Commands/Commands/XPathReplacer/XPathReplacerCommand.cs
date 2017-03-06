using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.XPathReplacer
{
    public class XPathReplacerCommand : LauncherCommand
    {
        public string FilePath { get; set; }

        public string XPath { get; set; }
        
        public string Value { get; set; }
    }
}