using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class UserService : IUserService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly INotificationService _notificationService;

        public UserService(IUnitOfWorkFactory unitOfWorkFactory, INotificationService notificationService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _notificationService = notificationService;
        }
        public async Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider, int offset, int count)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork socialNetworkData;

            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                socialNetworkData = GetSocialNetworkData(unitOfWork, provider);

                unitOfWork.Commit();
            }

            if (socialNetworkData == null)
            {
                throw new EndUserException("Can't get social network info with provider name = " + provider);
            }

            return await _socialNetworkApi.GetFriendsInfo(socialNetworkData.ThirdPartId, socialNetworkData.AccessToken, offset, count);
        }

        public async Task<List<TrackData.Audio>> GetFriendsTracks(string provider, string friendId)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork socialNetworkData;

            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                socialNetworkData = GetSocialNetworkData(unitOfWork, provider);

                unitOfWork.Commit();
            }

            if (socialNetworkData == null)
            {
                throw new EndUserException("Can't get social network info with provider name = " + provider);
            }

            return await _socialNetworkApi.GetTracks(friendId, socialNetworkData.AccessToken);
        }

        public User GetUserInfo(long id)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var user = unitOfWork.UserRepository.GetFullUserData(id);
                unitOfWork.Commit();
                return user;
            }
        }

        public User GetUserInfo(string email)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var user = unitOfWork.UserRepository.GetOne(u => u.Email == email);
                unitOfWork.Commit();
                return user;
            }
        }

        public User FollowPerson(long followerId)
        {
            return FollowOperation(followerId, true);
        }

        public User UnfollowPerson(long followerId)
        {
            return FollowOperation(followerId, false);
        }

        public async Task<List<UserDto>> SearchUsers(string searchText)
        {
            var usersDto = new List<UserDto>();

            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                List<User> users;
                if (searchText == "All")
                {
                    users = unitOfWork.UserRepository.GetAll().ToList();
                }
                else
                {
                    users = unitOfWork.UserRepository
                    .GetAll(user =>
                        (user.Name.FirstName.ToLower() + ' ' + user.Name.LastName.ToLower()).Contains(searchText)
                        || (user.Name.LastName.ToLower() + ' ' + user.Name.FirstName.ToLower()).Contains(searchText)).ToList();
                }

                Mapper.Map(users, usersDto);

                unitOfWork.Commit();
            }
            
            return usersDto;
        }

        private UserSocialNetwork GetSocialNetworkData(IUnitOfWork unitOfWork, string provider)
        {
            var userSocialNetworkRepo = unitOfWork.GetRepository<UserSocialNetwork>();
            return userSocialNetworkRepo.GetOne(
                s =>
                    (s.SocialNetwork.Name == provider) &&
                    (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));
        }

        private User FollowOperation(long followerId, bool isFollow)
        {
            User user;
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                user = unitOfWork.UserRepository.GetFullUserData(followerId);
                var loggedUser = unitOfWork.UserRepository.GetOne(x => x.Id == AzimuthIdentity.Current.UserCredential.Id);
                if (user == null || loggedUser == null)
                {
                    throw new EndUserException("Something wrong during unfollowing operation.");
                }
                Notifications notification;
                if (isFollow)
                {
                    user.Followers.Add(loggedUser);
                    notification = Notifications.Followed;
                }
                else
                {
                    user.Followers.Remove(loggedUser);
                    notification = Notifications.Unfollowed;
                }

                var notif = _notificationService.CreateNotification(notification, loggedUser, user);

                unitOfWork.NotificationRepository.AddItem(notif);

                unitOfWork.Commit();
            }

            return user;
        } 
    }
}