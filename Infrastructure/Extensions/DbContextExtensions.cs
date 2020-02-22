using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static string GetTableName(this DbContext dbContext, Type entityType)
        {
            var objectContextAdapter = (dbContext as IObjectContextAdapter);
            if (objectContextAdapter == null)
            {
                throw new ArgumentNullException("objectContextAdapter");
            }

            var objectContext = objectContextAdapter.ObjectContext;
            var metadataWorkspace = objectContext.MetadataWorkspace;

            var entitySetBase = metadataWorkspace.GetItemCollection(DataSpace.SSpace)
                                                 .GetItems<EntityContainer>()
                                                 .Single()
                                                 .BaseEntitySets.SingleOrDefault(x => x.Name == entityType.Name);

            return entitySetBase != null ? string.Concat(entitySetBase.MetadataProperties["Schema"].Value, ".", entitySetBase.MetadataProperties["Table"].Value) : null;
        }

        public static IDictionary<PropertyInfo, string> GetMappings(this DbContext dbContext, Type entityType)
        {
            var objectContextAdapter = (dbContext as IObjectContextAdapter);
            if (objectContextAdapter == null)
            {
                throw new ArgumentNullException("objectContextAdapter");
            }

            var objectContext = objectContextAdapter.ObjectContext;
            var metadataWorkspace = objectContext.MetadataWorkspace;

            var storageEntityType = metadataWorkspace.GetItems(DataSpace.SSpace)
                                                     .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                                                     .OfType<EntityType>()
                                                     .SingleOrDefault(x =>
                                                         x.Name == entityType.Name ||
                                                         (entityType.BaseType != null && x.Name == entityType.BaseType.Name)  // Considera herança entre objetos mapeados
                                                     );

            var objectEntityType = metadataWorkspace.GetItems(DataSpace.OSpace)
                                                    .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                                                    .OfType<EntityType>()
                                                    .SingleOrDefault(x => x.Name == entityType.Name);

            if (storageEntityType != null && objectEntityType != null)
            {
                return (storageEntityType.Properties.Select((edmProperty, idx) => new
                {
                    Property = entityType.GetProperty(objectEntityType.Members[idx].Name),
                    edmProperty.Name

                }).ToDictionary(x => x.Property, x => x.Name));
            }

            return null;
        }
    }
}