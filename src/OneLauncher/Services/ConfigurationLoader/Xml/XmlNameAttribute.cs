using System;

namespace OneLauncher.Services.ConfigurationLoader.Xml
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