using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.GraphQL
{
    public class GraphQLService : IDisposable
    {
        GraphQLClient client;

        public GraphQLService(string url)
        {
            client = new GraphQLClient(url);
        }

        public async Task<dynamic> PostAsync(string query, dynamic variables, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GraphQLRequest
                {
                    Query = query,
                    Variables = variables
                };

                var response = await client.PostAsync(request, cancellationToken);

                if (response.Errors != null && response.Errors.Length > 0 && response.Data == null)
                {
                    var message = string.Join(";", response.Errors.Select(x => x.Message));
                    throw new Exception(message);
                }

                return response.Data;
            }
            catch (Exception)
            {
                throw;
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
                    client.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}