using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Ado
{
    /// <summary>
    /// ADO.Net command executor
    /// </summary>
    public interface IAdoExecutor : IDisposable, ICloneable
    {
        /// <summary>
        /// Command execution timeout
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Get schema table from db
        /// </summary>
        DataTable GetSchemaTable(string tableName);

        /// <summary>
        /// Execute text query
        /// </summary>
        IDataReader Execute(string commandText);

        /// <summary>
        /// Execute list of commands without response
        /// </summary>
        void Execute(ICollection<IAdoCommand> adoCommands, bool useSingleTransaction = false);

        /// <summary>
        /// Command execute
        /// </summary>
        IDataReader Execute(IAdoCommand adoCommand);

        /// <summary>
        /// Create new ADO.Net command
        /// </summary>
        IAdoCommand CreateCommand();

        /// <summary>
        /// Execute bulk insert
        /// </summary>
        void BulkInsert(DataTable dataTable, int batchSize = 5000, SqlBulkCopyOptions? options = null);

        /// <summary>
        /// Execute batch update
        /// </summary>
        void BatchUpdate(DataTable dataTable, string commandText, int batchSize = 5000);
    }
}