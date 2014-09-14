using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
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

        public async Task<List<User>> SearchUsers(string searchText)
        {
            var users = new List<User>();
            string[] searchParams = searchText.Split(' ');

            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                if ((searchParams.Length == 1) && (searchParams.Length != 0))
                {
                    users.AddRange(unitOfWork.UserRepository.Get(s => (s.Name.FirstName.ToLower() + " " + s.Name.LastName.ToLower()).Contains(searchParams[0])).ToList());
                }
                else
                {
                    users.AddRange(unitOfWork.UserRepository.Get(s => (s.Name.FirstName.ToLower() + " " + s.Name.LastName.ToLower()).Contains(searchParams[0] + " " + searchParams[1])).ToList());
                    users.AddRange(unitOfWork.UserRepository.Get(s => (s.Name.LastName.ToLower() + " " + s.Name.FirstName.ToLower()).Contains(searchParams[0] + " " + searchParams[1])).ToList());
                    users = users.Distinct().ToList();
                }
                unitOfWork.Commit();
            }
            //users = _userRepository.Get(s => s.Name.FirstName.ToLower().Contains(searchText)).ToList();
            //users.AddRange(_userRepository.Get(s => s.Name.LastName.ToLower().Contains(searchText)).ToList());
            
            return users;
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
                var loggedUser = unitOfWork.UserRepository.GetOne(x => x.Email == AzimuthIdentity.Current.UserCredential.Email);
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