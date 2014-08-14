using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public class UserTracksService : IUserTracksService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWork _unitOfWork;
        public UserTracksService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TrackData>> GetTracks(string provider)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork user;

            using (_unitOfWork)
            {
                var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
                user = userSocialNetworkRepo.GetOne(
                    s =>
                        (s.SocialNetwork.Name == provider) &&
                        (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));

                _unitOfWork.Commit();
            }

            List<TrackData> tracks = null;
            try
            {
                tracks = await _socialNetworkApi.GetTracks(user.ThirdPartId, user.AccessToken);
            }
            catch (UserAuthorizationException exception)
            {

            }

            return tracks;
        }
    }
}