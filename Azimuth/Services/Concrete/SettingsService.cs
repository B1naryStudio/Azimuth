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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SettingsService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public SettingsViewModel GetUserSettings()
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _networkRepository = unitOfWork.GetRepository<SocialNetwork>();

                var snList = _networkRepository.GetAll().ToList();
                var user = unitOfWork.UserRepository.GetOne(x => x.Email == AzimuthIdentity.Current.UserCredential.Email);
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

                unitOfWork.Commit();
                return viewModel;
            }
        }
    }
}