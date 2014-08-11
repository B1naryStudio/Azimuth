using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Ninject;

namespace Azimuth.DataGenerator
{
    public class DataGenerator
    {
        private readonly IKernel _kernel;

        public DataGenerator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void GenerateData()
        {
            using (var unitOfWork = _kernel.Get<IUnitOfWork>())
            {
                var users = GetUsers();
                var sn = GetSocialNetworks();
                var userSNs = GetUserSocialNetworks();
                var artists = GetArtists();
                var albums = GetAlbums();
                var tracks = GetTracks();
                var playlists = GetPlaylists();

                userSNs[0].Identifier = new UserSNIdentifier
                {
                    SocialNetwork = sn[0],
                    User = users[0]
                };
                userSNs[1].Identifier = new UserSNIdentifier
                {
                    SocialNetwork = sn[1],
                    User = users[1]
                };
                userSNs[2].Identifier = new UserSNIdentifier
                {
                    SocialNetwork = sn[2],
                    User = users[2]
                };
                userSNs[3].Identifier = new UserSNIdentifier
                {
                    SocialNetwork = sn[3],
                    User = users[3]
                };

                users[0].SocialNetworks.Add(userSNs[0]);
                users[1].SocialNetworks.Add(userSNs[1]);
                users[2].SocialNetworks.Add(userSNs[2]);
                users[3].SocialNetworks.Add(userSNs[3]);

                playlists[0].Creator = users[0];
                playlists[1].Creator = users[1];
                playlists[2].Creator = users[2];
                playlists[3].Creator = users[3];


                artists[0].Albums.Add(albums[0]);
                artists[0].Albums.Add(albums[1]);
                artists[1].Albums.Add(albums[2]);
                artists[1].Albums.Add(albums[3]);
                albums[0].Artist = artists[0];
                albums[1].Artist = artists[0];
                albums[2].Artist = artists[1];
                albums[3].Artist = artists[1];

                albums[0].Tracks.Add(tracks[0]);
                albums[0].Tracks.Add(tracks[1]);
                albums[1].Tracks.Add(tracks[2]);
                albums[1].Tracks.Add(tracks[3]);
                tracks[0].Album = albums[0];
                tracks[1].Album = albums[0];
                tracks[2].Album = albums[1];
                tracks[3].Album = albums[1];

                tracks[0].Playlists.Add(playlists[0]);
                tracks[0].Playlists.Add(playlists[1]);
                tracks[1].Playlists.Add(playlists[1]);
                tracks[1].Playlists.Add(playlists[2]);
                tracks[2].Playlists.Add(playlists[2]);
                tracks[2].Playlists.Add(playlists[3]);

                var snRepo = unitOfWork.GetRepository<SocialNetwork>();
                foreach (var socialNetwork in sn)
                {
                    snRepo.AddItem(socialNetwork);
                }

                var userRepo = unitOfWork.GetRepository<User>();
                foreach (var user in users)
                {
                    userRepo.AddItem(user);
                }

                var userSnRepo = unitOfWork.GetRepository<UserSocialNetwork>();
                foreach (var userSocialNetwork in userSNs)
                {
                    userSnRepo.AddItem(userSocialNetwork);
                }

                var playlistRepo = unitOfWork.GetRepository<Playlist>();
                foreach (var playlist in playlists)
                {
                    playlistRepo.AddItem(playlist);
                }

                var artistRepo = unitOfWork.GetRepository<Artist>();
                foreach (var artist in artists)
                {
                    artistRepo.AddItem(artist);
                }

                unitOfWork.Commit();
            }
        }

        public void ClearDatabase()
        {
            using (var unitOfWork = _kernel.Get<IUnitOfWork>())
            {
                var artistsRepo = unitOfWork.GetRepository<Artist>();

                var artists = artistsRepo.GetAll();
                foreach (var track in artists)
                {
                    artistsRepo.DeleteItem(track);
                }

                var playlistRepo = unitOfWork.GetRepository<Playlist>();
                var playlists = playlistRepo.GetAll();
                foreach (var playlist in playlists)
                {
                    playlistRepo.DeleteItem(playlist);
                }

                var snRepo = unitOfWork.GetRepository<SocialNetwork>();
                var sns = snRepo.GetAll();
                foreach (var socialNetwork in sns)
                {
                    snRepo.DeleteItem(socialNetwork);
                }

                var userRepo = unitOfWork.GetRepository<User>();
                var users = userRepo.GetAll();
                foreach (var user in users)
                {
                    userRepo.DeleteItem(user);
                }

                

                unitOfWork.Commit();
            }
        }

        private List<Playlist> GetPlaylists()
        {
            var playlists = new List<Playlist>
            {
                new Playlist
                {
                    Name = "First playlist",
                    Accessibilty = "Private",
                },
                new Playlist
                {
                    Name = "Second playlist",
                    Accessibilty = "Private",
                },
                new Playlist
                {
                    Name = "Third playlist",
                    Accessibilty = "Private",
                },
                new Playlist
                {
                    Name = "Fourth playlist",
                    Accessibilty = "Private",
                },
            };
            return playlists;
        }

        private List<UserSocialNetwork> GetUserSocialNetworks()
        {
            var userSN = new List<UserSocialNetwork>
            {
                new UserSocialNetwork
                {
                    AccessToken = "asdfghjk4567fghj",
                    ThirdPartId = "drtfyguhj5678",
                    TokenExpires = "100"
                },
                new UserSocialNetwork
                {
                    AccessToken = "qwertyui56789",
                    ThirdPartId = "vbn3456vb",
                    TokenExpires = "200"
                },
                new UserSocialNetwork
                {
                    AccessToken = "lkjhgfd23456",
                    ThirdPartId = "dmnbv567",
                    TokenExpires = "300"
                },
                new UserSocialNetwork
                {
                    AccessToken = "hjklgfd7891",
                    ThirdPartId = "zxcmnbnm28",
                    TokenExpires = "400"
                },
            };
            return userSN;
        }

        private List<SocialNetwork> GetSocialNetworks()
        {
            var sn = new List<SocialNetwork>
            {
                new SocialNetwork
                {
                    Name = "Vkontakte"
                },
                new SocialNetwork
                {
                    Name = "Google"
                },
                new SocialNetwork
                {
                    Name = "Facebook"
                },
                new SocialNetwork
                {
                    Name = "Twitter"
                }
            };
            return sn;
        }

        private List<User> GetUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    Name = new Name {FirstName = "First", LastName = "User"},
                    ScreenName = "FirstUser",
                    Birthday = "10.10.2010",
                    Email = "first_user@gmail.com",
                    Gender = "Male",
                    Location = new Location {City = "Donetsk", Country = "Ukraine"},
                    Photo = "first_photo.jpg",
                    Timezone = 1
                },
                new User
                {
                    Name = new Name {FirstName = "Second", LastName = "User"},
                    ScreenName = "SecondUser",
                    Birthday = "10.10.2010",
                    Email = "second_user@gmail.com",
                    Gender = "Male",
                    Location = new Location {City = "Kharkov", Country = "Ukraine"},
                    Photo = "second_photo.jpg",
                    Timezone = 2
                },
                new User
                {
                    Name = new Name {FirstName = "Third", LastName = "User"},
                    ScreenName = "ThirdUser",
                    Birthday = "10.10.2010",
                    Email = "third_user@gmail.com",
                    Gender = "Male",
                    Location = new Location {City = "Kiev", Country = "Ukraine"},
                    Photo = "third_photo.jpg",
                    Timezone = 3
                },
                new User
                {
                    Name = new Name {FirstName = "Fourth", LastName = "User"},
                    ScreenName = "FourthUser",
                    Birthday = "10.10.2010",
                    Email = "fourth_user@gmail.com",
                    Gender = "Male",
                    Location = new Location {City = "Odessa", Country = "Ukraine"},
                    Photo = "fourth_photo.jpg",
                    Timezone = 4
                },
            };
            return users;
        }

        private List<Artist> GetArtists()
        {
            var artists = new List<Artist>
            {
                new Artist
                {
                    Description = "First artist",
                    Name = "FirstArtist",
                    Site = "www.first.com"
                },
                new Artist
                {
                    Description = "Second artist",
                    Name = "SecondArtist",
                    Site = "www.second.com"
                },
                new Artist
                {
                    Description = "Third artist",
                    Name = "ThirdArtist",
                    Site = "www.third.com"
                },
                new Artist
                {
                    Description = "Fourth artist",
                    Name = "FourthArtist",
                    Site = "www.Fourth.com"
                }
            };
            return artists;
        }

        private List<Album> GetAlbums()
        {
            var albums = new List<Album>
            {
                new Album
                {
                    Name = "FirstAlbum",
                    Description = "First Album"
                },
                new Album
                {
                    Name = "SecondAlbum",
                    Description = "Second Album"
                },
                new Album
                {
                    Name = "ThirdAlbum",
                    Description = "Third Album"
                },
                new Album
                {
                    Name = "FourthAlbum",
                    Description = "Fourth Album"
                }
            };
            return albums;
        }

        private List<Track> GetTracks()
        {
            var tracks = new List<Track>
            {
                new Track
                {
                    Lyrics = "First Track LA LA LA",
                    Duration = "10",
                    Genre = "Pop",
                    Name = "FirstTrack",
                    Url = "cs.first.mp3"
                },
                new Track
                {
                    Lyrics = "Second Track LA LA LA",
                    Duration = "20",
                    Genre = "Rock",
                    Name = "SecondTrack",
                    Url = "cs.second.mp3"
                },
                new Track
                {
                    Lyrics = "Third Track LA LA LA",
                    Duration = "30",
                    Genre = "Jazz",
                    Name = "ThirdTrack",
                    Url = "cs.third.mp3"
                },
                new Track
                {
                    Lyrics = "Fourth Track LA LA LA",
                    Duration = "40",
                    Genre = "Blues",
                    Name = "FourthTrack",
                    Url = "cs.fourth.mp3"
                }
            };
            return tracks;
        }
    }
}