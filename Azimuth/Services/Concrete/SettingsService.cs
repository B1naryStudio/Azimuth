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

        public SettingsViewModel GetUserSettings()
        {
            using (_unitOfWork)
            {
                var snList = _networkRepository.GetAll().ToList();
                var user = _userRepository.GetOne(x => x.Email == AzimuthIdentity.Current.UserCredential.Email);
                if (user == null || snList.Count == 0)
                {
                    throw new EndUserException("Can't get current user info");
                }
                var connectedSn = user.SocialNetworks.Select(x => x.SocialNetwork).ToList();
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