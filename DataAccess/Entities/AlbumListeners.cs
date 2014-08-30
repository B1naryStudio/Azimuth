using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    class AlbumListeners:BaseEntity
    {
        public virtual Album Album { get; set; }
        public virtual ICollection<User> Listeners { get; set; }
    }
}
