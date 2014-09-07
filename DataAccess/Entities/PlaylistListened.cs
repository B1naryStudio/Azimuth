using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class PlaylistListened:BaseEntity
    {
        public virtual Playlist Playlist { get; set; }
        public virtual int Amount { get; set; }
    }
}
