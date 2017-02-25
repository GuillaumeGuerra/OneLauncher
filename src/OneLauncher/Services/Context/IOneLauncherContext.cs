namespace OneLauncher.Services.Context
{
    public interface IOneLauncherContext
    {
        ApplicationSettings ApplicationSettings { get; }

        UserSettings UserSettings { get; }

        void SaveUserSettings();
    }
}