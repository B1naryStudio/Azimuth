using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public class UserTracksService : IUserTracksService
    {
        private readonly ISocialNetworkApi _socialNetworkApi;
        public UserTracksService(ISocialNetworkApi socialNetworkApi)
        {
            _socialNetworkApi = socialNetworkApi;
        }

        public async Task<List<TrackData>> GetTracks(string provider)
        {
            return await _socialNetworkApi.GetTracks(AzimuthIdentity.Current.UserCredential.SocialNetworkId,
                AzimuthIdentity.Current.UserCredential.AccessToken);
        }
    }
}