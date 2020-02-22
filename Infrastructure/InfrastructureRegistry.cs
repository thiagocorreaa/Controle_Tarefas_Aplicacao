using Infrastructure.Ado;
using Infrastructure.QueryBuilder;
using StructureMap;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace Infrastructure
{
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            this.For<IUnitOfWork>().Use<UnitOfWork>()
                .Ctor<DbContext>().Is<TarefasContext>().Named("TarefasContext");
                        
            For(typeof(IQuery<>)).Use(typeof(Query<>));

            this.For<IAdoExecutor>().Use<AdoExecutor>()
                .Ctor<IDbConnection>("sqlConnection")
                .Is((ctx) => ConnectionFactory("TarefasContext", ctx));
        }

        static IDbConnection ConnectionFactory(string connectionName, IContext structureMapContext)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionName];

            if (string.IsNullOrWhiteSpace(connectionName) || connectionString == null || string.IsNullOrWhiteSpace(connectionString.ProviderName) || string.IsNullOrWhiteSpace(connectionString.ConnectionString))
            {
                var msg = string.Format("Undefined ConnectionString '{0}'. Check your config file", connectionName);
                throw new ArgumentNullException(msg);
            }

            var factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            var connection = factory.CreateConnection();

            if (connection != null)
            {
                connection.ConnectionString = connectionString.ConnectionString;
            }

            return connection;
        }
    }
}