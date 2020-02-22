using Infrastructure.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    class DynamicRepository : IDynamicRepository
    {
        static readonly TimeSpan RECURSIVE_LOAD_TIMEOUT;

        static DynamicRepository()
        {
            if (Debugger.IsAttached)
            {
                RECURSIVE_LOAD_TIMEOUT = Timeout.InfiniteTimeSpan;
            }
            else
            {
                RECURSIVE_LOAD_TIMEOUT = TimeSpan.FromMinutes(5);
            }
        }

        DbContext DbContext { get; set; }

        DbSet DbSet { get; set; }

        Type CurrentType { get; set; }

        public DynamicRepository(DbContext dbContext, Type entitySetType)
        {
            if (entitySetType == null)
            {
                throw new ArgumentNullException("entitySetType cannot be null.");
            }

            if (dbContext == null)
            {
                throw new ArgumentNullException("dbContext cannot be null.");
            }

            CurrentType = entitySetType;
            DbContext = dbContext;
            DbSet = dbContext.Set(entitySetType);
        }

        public IAttachedObject Find(object[] keys, ushort loadGraphDepth = 0)
        {
            object entity;

            lock (DbContext)
                entity = DbSet.Find(keys);


            using (var tokenSrc = new CancellationTokenSource(RECURSIVE_LOAD_TIMEOUT))
            {
                var task = Task.Run(() => LoadGraph(entity, CurrentType, loadGraphDepth, DbContext, tokenSrc.Token), tokenSrc.Token);

                if (!task.Wait(RECURSIVE_LOAD_TIMEOUT))
                {
                    throw new OperationCanceledException("Couldn't load data graph");
                }
            }

            return new AttachedObject(DbContext, entity);
        }

        public virtual object Add(object entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                entity = DbSet.Add(entity);
            }

            return entity;
        }

        public virtual object Update(object entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            dbEntityEntry.State = EntityState.Modified;

            return entity;
        }

        public virtual object Delete(object entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                entity = DbSet.Attach(entity);
            }

            entity = DbSet.Remove(entity);

            return entity;
        }

        public string GetTableName() => DbContext.GetTableName(CurrentType);

        public IDictionary<PropertyInfo, string> GetMappings() => DbContext.GetMappings(CurrentType);

        static void LoadGraph(object instance, Type instanceType, ushort currentDepth, DbContext dbContext, CancellationToken abort)
        {
            if (currentDepth <= 0 || instance == null || abort.IsCancellationRequested)
            {
                return;
            }

            var baseCollectionType = typeof(ICollection<>);
            var ps = instanceType.GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetMethod.IsVirtual && p.SetMethod.IsVirtual).ToArray();

            if (ps.Length == 0)
            {
                return;
            }

            currentDepth--;

            foreach (var prop in ps)
            {
                abort.ThrowIfCancellationRequested();

                var pType = prop.PropertyType;
                var isCollection = pType.IsGenericType && baseCollectionType.IsAssignableFrom(pType.GetGenericTypeDefinition());
                var propValue = prop.GetValue(instance);

                // se o objeto já foi carregado ou se a coleção não estiver vazia, CAI FORA!
                if (propValue != null && (!isCollection || ((ICollection)propValue).Count > 0))
                {
                    continue;
                }

                if (isCollection)
                {
                    lock (dbContext)
                        dbContext.Entry(instance).Collection(prop.Name).Load();


                    propValue = prop.GetValue(instance);
                    var containedType = prop.PropertyType.GetGenericArguments().Single();

                    foreach (var item in ((ICollection)propValue))
                        LoadGraph(item, containedType, currentDepth, dbContext, abort);

                }
                else
                {
                    dbContext.Entry(instance).Reference(prop.Name).Load();
                    propValue = prop.GetValue(instance);

                    lock (dbContext)
                    {
                        LoadGraph(propValue, prop.PropertyType, currentDepth, dbContext, abort);
                    }
                }
            }
        }

        #region IDisposable Members

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Não dar dispose no context, uma vez que vários repositories podem estar usando o mesmo context
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}