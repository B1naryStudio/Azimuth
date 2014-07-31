using System;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Shared.Dto;
using Ninject;

namespace Azimuth.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new DataAccessModule());

            User user = null;
            using (var unitOfWork = kernel.Get<IUnitOfWork>())
            {
                IRepository<User> userRepo = unitOfWork.GetRepository<User>();

                user = userRepo.Get(1);
                //user = userRepo.Get(x => x.Email.StartsWith("test")).First();
                //user = userRepo.GetAll().First();

                unitOfWork.Commit();
            }

            UserBrief dto = new UserBrief
            {
                Name = user.DisplayName,
                Email = user.Email
            };

            Console.WriteLine("{0} {1}", dto.Name, dto.Email);
        }
    }
}
