using System;
using System.Threading.Tasks;

namespace BuildingBlock.CacheDistribuido.Services
{
    public interface IDistributedCaching
    {
        Task<String> ValidationCachedTimeExpires();

        void NewCachedTimeExpered();

        Task InsertCache(object cache);

        object GetCache(int Contador);

        Task InsertList(object[] listCache);

        T GetList<T>() where T : class, new();
    }
}
