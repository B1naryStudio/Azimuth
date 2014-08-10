using Azimuth.ViewModels;

namespace Azimuth.Services
{
    public interface ISettingsService
    {
        SettingsViewModel GetUserSettings();
    }
}