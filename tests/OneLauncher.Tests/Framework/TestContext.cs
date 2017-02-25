using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneLauncher.Services.Context;

namespace OneLauncher.Tests.Framework
{
    public class OneLaunchContextMock : IOneLauncherContext
    {
        public ApplicationSettings ApplicationSettings { get; set; }
        public UserSettings UserSettings { get; set; }

        public void SaveUserSettings()
        {
            throw new NotImplementedException();
        }
    }
}
