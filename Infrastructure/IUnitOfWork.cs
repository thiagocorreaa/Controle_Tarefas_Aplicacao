using Infrastructure.Repositories;
using System;

namespace Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        TimeSpan? Timeout { get; set; }
        T CustomRepository<T>() where T : class;
        IRepository<T> Repository<T>() where T : class;
        IDynamicRepository Repository(Type objectType);
        void SaveChanges();
        void DiscardChanges();
    }
}