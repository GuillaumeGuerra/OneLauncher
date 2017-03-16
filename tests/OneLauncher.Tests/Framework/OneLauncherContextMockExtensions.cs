using OneLauncher.Services.Context;

namespace OneLauncher.Tests.Framework
{
    public static class OneLauncherContextMockExtensions
    {
        public static OneLaunchContextMock WithUserSettingsDirectory(this OneLaunchContextMock context, string directory)
        {
            if (context.ApplicationSettings == null)
                context.ApplicationSettings = new ApplicationSettings();

            context.ApplicationSettings.UserSettingsDirectory = directory;

            return context;
        }

        public static OneLaunchContextMock WithUserSettings(this OneLaunchContextMock context, UserSettings settings)
        {
            context.UserSettings = settings;

            return context;
        }
    }
}