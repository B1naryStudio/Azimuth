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

            var dg = new DataGenerator.DataGenerator(kernel);
            dg.GenerateData();
            //dg.ClearDatabase();
            using (var unitOfWork = kernel.Get<IUnitOfWork>())
            {
                IRepository<Artist> artistRepository = unitOfWork.GetRepository<Artist>();

                var artist = artistRepository.GetAll();

                IRepository<Album> albumRepository = unitOfWork.GetRepository<Album>();
                var alb = albumRepository.Get(1);

                IRepository<Playlist> playRepository = unitOfWork.GetRepository<Playlist>();
                var pl = playRepository.GetAll();

                var trackRepo = unitOfWork.GetRepository<Track>();
                var tracks = trackRepo.GetAll();
                unitOfWork.Commit();
            }

           

            //User user = null;
            //using (var unitOfWork = kernel.Get<IUnitOfWork>())
            //{
            //    IRepository<User> userRepo = unitOfWork.GetRepository<User>();

            //    user = userRepo.Get(1);
            //    //user = userRepo.Get(x => x.Email.StartsWith("test")).First();
            //    //user = userRepo.GetAll().First();

            //    unitOfWork.Commit();
            //}

            //UserBrief dto = new UserBrief
            //{
            //    Name = user.ScreenName,
            //    Email = user.Email
            //};

            //Console.WriteLine("{0} {1}", dto.Name, dto.Email);
        }
    }
}
