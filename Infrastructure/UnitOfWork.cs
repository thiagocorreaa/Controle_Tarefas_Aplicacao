using Infrastructure.Repositories;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromMinutes(5);

        DbContext DbContext { get; set; }

        ConcurrentDictionary<string, IRepository> Repositories { get; set; }

        public TimeSpan? Timeout
        {
            get { return DbContext.Database.CommandTimeout.HasValue ? new TimeSpan?(TimeSpan.FromSeconds(DbContext.Database.CommandTimeout.Value)) : null; }
            set { DbContext.Database.CommandTimeout = value.HasValue ? new int?(Convert.ToInt32(value.Value.TotalSeconds)) : null; }
        }

        /// <param name="dbContext">Injetado pelo structuremap</param>
        public UnitOfWork(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException("dbContext", "Db Context cannot be null.");
            Repositories = new ConcurrentDictionary<string, IRepository>();
            Timeout = DEFAULT_TIMEOUT;
        }

        /// <typeparam name="T">Interface custom do repositório</typeparam>
        public T CustomRepository<T>() where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Generic type should be an interface.");
            }

            Func<DbContext, Type, IRepository> factory = (dbContext, type) =>
            {
                return (IRepository)AppDomain.CurrentDomain.GetAssemblies()
                                             .SelectMany(t => t.GetTypes())
                                             .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                                             .Select(x => Activator.CreateInstance(x, dbContext))
                                             .SingleOrDefault();
            };

            return (T)GetRepository(typeof(T), factory, "CUSTOM");
        }

        public IRepository<T> Repository<T>() where T : class
        {
            Func<DbContext, Type, IRepository> factory = (dbContext, type) => new Repository<T>(dbContext);

            return (IRepository<T>)GetRepository(typeof(T), factory, "GENERIC");
        }

        public IDynamicRepository Repository(Type objectType)
        {
            Func<DbContext, Type, IRepository> factory = (dbContext, type) => new DynamicRepository(dbContext, type);

            return (IDynamicRepository)GetRepository(objectType, factory, "DYNAMIC");
        }

        /// <param name="prefix">Como o dictionary cachea os IRepository por tipo, prevê um prefixo para evitar chaves duplicadas</param>
        IRepository GetRepository(Type objectType, Func<DbContext, Type, IRepository> repositoryFactory, string prefix)
        {
            IRepository repository;

            var typeName = prefix + "_" + objectType.FullName;

            if (!Repositories.TryGetValue(typeName, out repository))
            {
                repository = repositoryFactory.Invoke(DbContext, objectType);

                Repositories[typeName] = repository;
            }

            return repository;
        }

        public void SaveChanges()
        {
            try
            {
                DbContext.Configuration.AutoDetectChangesEnabled = true;

                if (!DbContext.ChangeTracker.HasChanges())
                {
                    return;
                }

                var errors = DbContext.GetValidationErrors();

                if (errors.Any())
                {
                    var sb = new StringBuilder();

                    foreach (var error in errors.Where(v => !v.IsValid))
                    {
                        // Remove o objeto do contexto
                        error.Entry.State = EntityState.Detached;

                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", error.Entry.Entity.GetType(), error.Entry.State));

                        foreach (var ve in error.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        }
                    }

                    throw new DbEntityValidationException(sb.ToString(), errors);
                }

                DbContext.SaveChanges();
            }
            finally
            {
                DbContext.Configuration.AutoDetectChangesEnabled = false;
            }
        }

        public void DiscardChanges()
        {
            var dbEntityEntries = DbContext.ChangeTracker.Entries();

            foreach (var dbEntityEntry in dbEntityEntries)
                dbEntityEntry.State = EntityState.Detached;

        }

        #region IDisposable Members

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    var connection = DbContext.Database.Connection;
                    if (connection != null && connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }

                    if (DbContext != null)
                    {
                        DbContext.Dispose();
                        DbContext = null;
                    }

                    if (Repositories != null)
                    {
                        foreach (var repository in Repositories.Values)
                            repository.Dispose();


                        Repositories.Clear();
                        Repositories = null;
                    }
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