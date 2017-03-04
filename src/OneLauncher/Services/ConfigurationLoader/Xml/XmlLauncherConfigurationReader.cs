using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OneLauncher.Framework;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlLauncherConfigurationReader : IXmlLauncherConfigurationReader
    {
        public XmlLauncherConfiguration LoadFile(string filePath)
        {
            var xOver = new XmlAttributeOverrides();
            var xAttrs = new XmlAttributes();

            foreach (var type in App.Container.GetRegisteredSubClasses<XmlLauncherCommand>())
            {
                var attribute = type.GetCustomAttributes(typeof(XmlNameAttribute), false);
                if (attribute.Length > 0)
                    xAttrs.XmlArrayItems.Add(new XmlArrayItemAttribute(((XmlNameAttribute)attribute[0]).Name, type));
            }
            
            xOver.Add(typeof(XmlLauncherLink), "Commands", xAttrs);

            XmlSerializer serializer = new XmlSerializer(typeof(XmlLauncherConfiguration), xOver);

            if (!File.Exists(filePath))
                throw new InvalidOperationException(string.Format("Unknow file path [{0}]", filePath));

            var text = File.ReadAllText(filePath);
            using (var reader = new StringReader(text))
            {
                return (XmlLauncherConfiguration)serializer.Deserialize(reader);
            }
        }
    }
}