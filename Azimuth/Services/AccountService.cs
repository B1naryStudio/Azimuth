﻿using System;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure;
using Iesi.Collections.Generic;

namespace Azimuth.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly UserSocialNetworkRepository _userSNRepository;
        private readonly SocialNetworkRepository _snRepository;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Get repositories
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _userSNRepository = _unitOfWork.GetRepository<UserSocialNetwork>() as UserSocialNetworkRepository;
            _snRepository = _unitOfWork.GetRepository<SocialNetwork>() as SocialNetworkRepository;
        }

        public bool SaveOrUpdateUserData(User user, UserCredential userCredential, AzimuthIdentity loggedIdentity)
        {
            using (_unitOfWork)
            {
                try
                {
                    User loggedUser = null;
                    if (loggedIdentity != null)
                    {
                        loggedUser = _userRepository.GetOne(x => x.Email == loggedIdentity.UserCredential.Email);
                    }
                    var userSn = _userSNRepository.GetByThirdPartyId(userCredential.SocialNetworkId);
                    if (userSn != null)
                    {
                        if (loggedUser != null)
                        {
//                            var userToDelete = userSn.Identifier.User;
//                            userToDelete.SocialNetworks.Clear();
                            userSn.User = loggedUser; // TODO Resolve issue with composite update
//                            _userRepository.Remove(userToDelete);
                        }
                        else
                        {
                            // If user exists in database check his data fields for updating
                            if (user.ToString() != userSn.User.ToString())
                            {
                                userSn.User.Name = new Name
                                {
                                    FirstName = user.Name.FirstName,
                                    LastName = user.Name.LastName
                                };
                                userSn.User.ScreenName = user.ScreenName;
                                userSn.User.Gender = user.Gender;
                                userSn.User.Email = user.Email;
                                userSn.User.Birthday = user.Birthday;
                                userSn.User.Location = new Location
                                {
                                    Country = user.Location.Country,
                                    City = user.Location.City
                                };
                                userSn.User.Timezone = user.Timezone;
                                userSn.User.Photo = userSn.Photo = user.Photo;
                                userSn.UserName = (user.Name.FirstName ??
                                    String.Empty) + "" + ((user.Name.LastName != null) ? (" " + user.Name.LastName) : String.Empty);
                            }
                        }
                    }
                    else
                    {
                        var currentSN = _snRepository.GetByName(userCredential.SocialNetworkName);

                        if (loggedIdentity == null)
                        {
                            _userRepository.AddItem(user);
                        }
                        _userSNRepository.AddItem(new UserSocialNetwork
                        {
                            User = loggedUser ?? user, 
                            SocialNetwork = currentSN,
                            ThirdPartId = userCredential.SocialNetworkId,
                            AccessToken = userCredential.AccessToken,
                            TokenExpires = userCredential.AccessTokenExpiresIn,
                            Photo = user.Photo,
                            UserName = user.Name.FirstName ?? String.Empty + user.Name.LastName ?? String.Empty
                        });
                    }

                    _unitOfWork.Commit();
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DisconnectUserAccount(string provider)
        {
            using (_unitOfWork)
            {
                try
                {
                    var user = _userRepository.GetOne(x => x.Email == AzimuthIdentity.Current.UserCredential.Email);
                    var socialNetwork = _snRepository.GetOne(x => x.Name == provider);
                    if (user == null || socialNetwork == null)
                    {
                        throw new ApplicationException(
                            string.Format("Can't find user or social network (email: {0}, SN name: {1}",
                                AzimuthIdentity.Current.UserCredential.Email, provider));
                    }
                    _userSNRepository.Remove(user.Id, socialNetwork.Id);
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}