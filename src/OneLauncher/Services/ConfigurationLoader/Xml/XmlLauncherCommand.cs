﻿using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    [XmlInclude(typeof(XmlExecuteCommand))]
    [XmlInclude(typeof(XmlXPathReplacerCommand))]
    public abstract class XmlLauncherCommand
    {
        protected const string RootToken = "[ROOT]";

        public abstract LauncherCommand ToCommand(string resolvedRootPath);
    }
}