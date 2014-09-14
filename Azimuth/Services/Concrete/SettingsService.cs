using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
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

        public SettingsViewModel GetUserSettings(long? id)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var networkRepository = unitOfWork.GetRepository<SocialNetwork>();

                var snList = networkRepository.GetAll().ToList();
                var user = unitOfWork.UserRepository.GetOne(x => x.Id == (id ?? AzimuthIdentity.Current.UserCredential.Id));
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

                unitOfWork.Commit();
                return viewModel;
            }
        }
    }
}