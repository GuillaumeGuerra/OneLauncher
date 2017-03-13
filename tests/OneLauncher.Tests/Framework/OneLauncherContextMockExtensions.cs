using OneLauncher.Services.Context;

namespace OneLauncher.Tests.Framework
{
    public static class OneLauncherContextMockExtensions
    {
        public static OneLaunchContextMock WithUserSettingsDirectory(this OneLaunchContextMock mock, string directory)
        {
            if(mock.ApplicationSettings==null)
                mock.ApplicationSettings=new ApplicationSettings();

            mock.ApplicationSettings.UserSettingsDirectory = directory;

            return mock;
        }
    }
}