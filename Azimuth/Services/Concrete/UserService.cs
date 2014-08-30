using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;

namespace Azimuth.Services.Concrete
{
    public class UserService : IUserService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork socialNetworkData;

            using (_unitOfWork)
            {
                socialNetworkData = GetSocialNetworkData(provider);

                _unitOfWork.Commit();
            }

            if (socialNetworkData == null)
            {
                throw new EndUserException("Can't get social network info with provider name = " + provider);
            }

            return await _socialNetworkApi.GetFriendsInfo(socialNetworkData.ThirdPartId, socialNetworkData.AccessToken);
        }

        public async Task<List<TrackData.Audio>> GetFriendsTracks(string provider, string friendId)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork socialNetworkData;

            using (_unitOfWork)
            {
                socialNetworkData = GetSocialNetworkData(provider);

                _unitOfWork.Commit();
            }

            if (socialNetworkData == null)
            {
                throw new EndUserException("Can't get social network info with provider name = " + provider);
            }

            return await _socialNetworkApi.GetTracks(friendId, socialNetworkData.AccessToken);
        }

        private UserSocialNetwork GetSocialNetworkData(string provider)
        {
            var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
            return userSocialNetworkRepo.GetOne(
                s =>
                    (s.SocialNetwork.Name == provider) &&
                    (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));
        }
    }
}