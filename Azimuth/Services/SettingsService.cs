using System.Security.Claims;
using System.Threading;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure;
using Azimuth.Models;
using Azimuth.ViewModels;

namespace Azimuth.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
        }

        public SettingsViewModel GetUserSettings()
        {
            using (_unitOfWork)
            {
                var identity = ClaimsIdentity as AzimuthIdentity;
                
                var viewModel = new SettingsViewModel
                {
                    User = UserModel.From(_userRepository.GetUserByEmail(identity.UserCredential.Email))
                };
                return viewModel;
            }
        }
    }
}