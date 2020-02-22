using System;
using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.Repositories
{
    public interface IDynamicRepository : IRepository
    {
        IAttachedObject Find(object[] keys, ushort loadGraphDepth = 0);
        object Add(object entity);
        object Update(object entity);
        object Delete(object entity);
        string GetTableName();
        IDictionary<PropertyInfo, string> GetMappings();
    }

    public interface IAttachedObject : IDisposable
    {
        object Instance { get; }

        object Detach();
    }
}