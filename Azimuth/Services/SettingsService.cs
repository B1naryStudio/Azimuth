﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Models;
using Azimuth.ViewModels;

namespace Azimuth.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly SocialNetworkRepository _networkRepository;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _networkRepository = _unitOfWork.GetRepository<SocialNetwork>() as SocialNetworkRepository;
        }

        public SettingsViewModel GetUserSettings()
        {
            using (_unitOfWork)
            {
                var snList = _networkRepository.GetAll().ToList();
                var user = _userRepository.GetUserByEmail("killer-korsar@yandex.ru");  // TODO Here should be logged user email
                var connectedSn = user.SocialNetworks.Select(x => x.Identifier.SocialNetwork).ToList();
                var availableSn = snList.Except(connectedSn).ToList();
                var viewModel = new SettingsViewModel
                {
                    User = UserModel.From(user),
                    AvailableNetworks = availableSn,
                    ConnectedNetworks = connectedSn
                };
                return viewModel;
            }
        }
    }
}