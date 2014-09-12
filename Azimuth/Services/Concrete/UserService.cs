﻿using System.Collections.Generic;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public UserService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _notificationService = notificationService;
        }
        public async Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider, int offset, int count)
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

            return await _socialNetworkApi.GetFriendsInfo(socialNetworkData.ThirdPartId, socialNetworkData.AccessToken, offset, count);
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

        public User GetUserInfo(long id)
        {
            using (_unitOfWork)
            {
                var user = _userRepository.GetFullUserData(id);
                _unitOfWork.Commit();
                return user;
            }
        }

        public User GetUserInfo(string email)
        {
            using (_unitOfWork)
            {
                var user = _userRepository.GetOne(u => u.Email == email);
                _unitOfWork.Commit();
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

        private UserSocialNetwork GetSocialNetworkData(string provider)
        {
            var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
            return userSocialNetworkRepo.GetOne(
                s =>
                    (s.SocialNetwork.Name == provider) &&
                    (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));
        }

        private User FollowOperation(long followerId, bool isFollow)
        {
            User user;
            User loggedUser;
            Notifications notification;
            using (_unitOfWork)
            {
                user = _userRepository.GetFullUserData(followerId);
                loggedUser = _userRepository.GetOne(x => x.Email == AzimuthIdentity.Current.UserCredential.Email);
                if (user == null || loggedUser == null)
                {
                    throw new EndUserException("Something wrong during unfollowing operation.");
                }
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
                
                _unitOfWork.Commit();
            }

            _notificationService.CreateNotification(notification, loggedUser, user);
            return user;
        }
    }
}