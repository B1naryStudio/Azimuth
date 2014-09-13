using Azimuth.ViewModels;

namespace Azimuth.Services.Interfaces
{
    public interface ISettingsService
    {
        SettingsViewModel GetUserSettings(long? id);
    }
}