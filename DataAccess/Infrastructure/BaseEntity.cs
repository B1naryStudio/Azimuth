
namespace Azimuth.DataAccess.Infrastructure
{
    public abstract class BaseEntity : IEntity
    {
        public virtual long Id { get; set; }
    }
}
