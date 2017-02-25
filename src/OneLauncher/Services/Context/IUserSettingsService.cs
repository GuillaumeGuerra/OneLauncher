namespace OneLauncher.Services.Context
{
    public interface IUserSettingsService
    {
        UserSettings GetUserSettings();

        void SaveUserSettings(UserSettings settings);
    }
}