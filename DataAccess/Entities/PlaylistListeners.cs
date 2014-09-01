using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class PlaylistListeners:BaseEntity
    {
        public virtual Playlist Playlist { get; set; }
        public virtual User Listener { get; set; }
        public override string ToString()
        {
            return String.Format("PlaylistId = {0}:ListenerId = {1}", Playlist.Id, Listener.Id);
        }
    }
}
