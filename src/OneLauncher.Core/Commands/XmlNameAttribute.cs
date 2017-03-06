using System;

namespace OneLauncher.Core.Commands
{
    public class XmlNameAttribute : Attribute
    {
        public string Name { get; }

        public XmlNameAttribute(string name)
        {
            Name = name;
        }
    }
}