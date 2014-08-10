using System.Collections.Generic;
using Azimuth.DataAccess.Entities;
using Azimuth.Models;

namespace Azimuth.ViewModels
{
    public class SettingsViewModel
    {
        public UserModel User { get; set; }
        public List<SocialNetwork> AvailableNetworks { get; set; }
    }
}