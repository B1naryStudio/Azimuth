using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Models;
using Azimuth.Services.Interfaces;
using Azimuth.ViewModels;

namespace Azimuth.Services.Concrete
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

        public SettingsViewModel GetUserSettings(long? id)
        {
            using (_unitOfWork)
            {
                var snList = _networkRepository.GetAll().ToList();
                var user = _userRepository.GetOne(x => x.Id == (id ?? AzimuthIdentity.Current.UserCredential.Id));
                if (user == null || snList.Count == 0)
                {
                    throw new EndUserException("Can't get current user info");
                }
                var connectedSn = user.SocialNetworks.Select(x => x.SocialNetwork).ToList();
                var socialNetworkInfo = new List<SocialNetworkInfo>();
                snList.ForEach(x => socialNetworkInfo.Add(new SocialNetworkInfo
                {
                    SocialNetwork = x,
                    IsConnected = connectedSn.Contains(x)
                }));

                var viewModel = new SettingsViewModel
                {
                    User = UserModel.From(user),
                    SocialNetworks = socialNetworkInfo
                };

                _unitOfWork.Commit();
                return viewModel;
            }
        }
    }
}