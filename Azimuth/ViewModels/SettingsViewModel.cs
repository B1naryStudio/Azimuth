using System.Collections.Generic;
using Azimuth.DataAccess.Entities;
using Azimuth.Models;

namespace Azimuth.ViewModels
{
    public class SettingsViewModel
    {
        public UserModel User { get; set; }
        public List<SocialNetworkInfo> SocialNetworks { get; set; }
    }

    public class SocialNetworkInfo
    {
        public SocialNetwork SocialNetwork { get; set; }
        public bool IsConnected { get; set; }
    }
}