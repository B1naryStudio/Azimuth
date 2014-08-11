using System;
using System.Collections.Generic;

namespace Azimuth.DataAccess.Infrastructure
{
    public interface IRepository<T> where T : IEntity
    {
        T Get(long id);
        IEnumerable<T> Get(Func<T, Boolean> filter);
        T GetOne(Func<T, Boolean> filter);
        IEnumerable<T> GetAll();
        void AddItem(T item);

        void DeleteItem(T item);
    }
}
