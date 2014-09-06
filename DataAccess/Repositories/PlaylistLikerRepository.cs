﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class PlaylistLikerRepository:BaseRepository<PlaylistLike>
    {
        public PlaylistLikerRepository(ISession session) : base(session)
        {
        }
    }
}
