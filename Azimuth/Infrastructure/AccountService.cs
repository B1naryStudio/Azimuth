﻿using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.Infrastructure
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IRepository<User> _userRepository;
        private IRepository<UserSocialNetwork> _userSNRepository;
        private IRepository<SocialNetwork> _snRepository;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool SaveOrUpdateUserData(User user, string socialId, string socialNetwork, string accessToken, string tokenExpiresIn)
        {
            using (_unitOfWork)
            {
                try
                {
                    // Get repositories
                    _userRepository = _unitOfWork.GetRepository<User>();
                    _userSNRepository = _unitOfWork.GetRepository<UserSocialNetwork>();
                    _snRepository = _unitOfWork.GetRepository<SocialNetwork>();

                    // Check wheter current user exists in database
                    var getUserFromDB =
                        _userSNRepository.Get(s => s.ThirdPartId == socialId).ToList();
                    if (getUserFromDB.Count != 0)
                    {
                        // If user exists in database check his data fields for updating
                        if (user.ToString() != getUserFromDB[0].Identifier.User.ToString())
                        {
                            getUserFromDB[0].Identifier.User.Name = new Name { FirstName = user.Name.FirstName, LastName = user.Name.LastName };
                            getUserFromDB[0].Identifier.User.ScreenName = user.ScreenName;
                            getUserFromDB[0].Identifier.User.Gender = user.Gender;
                            getUserFromDB[0].Identifier.User.Email = user.Email;
                            getUserFromDB[0].Identifier.User.Birthday = user.Birthday;
                            getUserFromDB[0].Identifier.User.Location = new Location { Country = user.Location.Country, City = user.Location.City };
                            getUserFromDB[0].Identifier.User.Timezone = user.Timezone;
                            getUserFromDB[0].Identifier.User.Photo = user.Photo;
                        }
                    }
                    else
                    {
                        var currentSN = _snRepository.Get(s => s.Name == socialNetwork).ToList();

                        _userRepository.AddItem(user);
                        _userSNRepository.AddItem(new UserSocialNetwork
                        {
                            Identifier = new UserSNIdentifier {User = user, SocialNetwork = currentSN[0]},
                            ThirdPartId = socialId,
                            AccessToken = accessToken,
                            TokenExpires = tokenExpiresIn
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
    }

    public interface IAccountService
    {
        bool SaveOrUpdateUserData(User user, string socialId, string socialNetwork, string accessToken, string tokenExpiresIn);
    }
}