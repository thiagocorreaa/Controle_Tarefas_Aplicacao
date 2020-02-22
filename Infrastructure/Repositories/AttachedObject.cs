using System;
using System.Data.Entity;

namespace Infrastructure.Repositories
{
    class AttachedObject : IAttachedObject
    {
        public DbContext DbContext { get; private set; }

        public object Instance { get; private set; }

        public AttachedObject(DbContext sourceContext, object entity)
        {
            DbContext = sourceContext;
            Instance = entity;
        }

        public object Detach()
        {
            if (Instance != null)
            {
                var entry = DbContext.Entry(Instance);
                entry.State = EntityState.Detached;
            }

            return Instance;
        }

        #region IDisposable Members

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Instance != null)
                    {
                        try
                        {
                            Detach();
                        }
                        catch (Exception) { }

                        Instance = null;
                    }

                    DbContext = null;
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