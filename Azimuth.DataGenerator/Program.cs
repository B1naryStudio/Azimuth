using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Shared.Enums;
using Ninject;

namespace Azimuth.DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            

            IKernel kernel = new StandardKernel(new DataAccessModule());

            var dg = new DataGenerator(kernel);

            //dg.GenerateData();
            using (var unitOfWork = kernel.Get<IUnitOfWork>())
            {
                IRepository<Artist> artistRepository = unitOfWork.GetRepository<Artist>();
                var playlistRepo = unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
                var trackRepo = unitOfWork.GetRepository<Track>();
                var tracks = trackRepo.GetAll();
                var userRepo = unitOfWork.GetRepository<User>();
                var user = userRepo.Get(49);
                playlistRepo.AddItem(new Playlist
                {
                    Accessibilty = Accessibilty.Public,
                    Creator = user,
                    Name = "First1",
                    Tracks = tracks.Where(tr => tr.Id % 4 != 0).ToList()
                });

                playlistRepo.AddItem(new Playlist
                {
                    Accessibilty = Accessibilty.Public,
                    Creator = user,
                    Name = "Second1",
                    Tracks = tracks.Where(tr => tr.Id % 2 != 0).ToList()
                });

                playlistRepo.AddItem(new Playlist
                {
                    Accessibilty = Accessibilty.Public,
                    Creator = user,
                    Name = "Third1",
                    Tracks = tracks.Where(tr => tr.Id % 2 != 0).ToList()
                });
                var artist = artistRepository.GetAll();

                IRepository<Album> albumRepository = unitOfWork.GetRepository<Album>();
                var alb = albumRepository.Get(1);

                IRepository<Playlist> playRepository = unitOfWork.GetRepository<Playlist>();
                var pl = playRepository.GetAll();

                
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
