using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Ado
{
    class AdoExecutor : IAdoExecutor, ICloneable
    {
        static readonly Regex PARAMETERS_REGEX = new Regex(@"\@\w+", RegexOptions.Compiled);
        static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromMinutes(1);

        protected SqlConnection SqlConnection { get; set; }
        public TimeSpan Timeout { get; set; }

        public AdoExecutor(IDbConnection sqlConnection)
        {
            if (sqlConnection == null)
            {
                throw new ArgumentNullException("Connections are null, please check constructor arguments");
            }

            if (!(sqlConnection is SqlConnection))
            {
                throw new ArgumentOutOfRangeException("Expected SqlConnection", "sqlConnection");
            }

            SqlConnection = (SqlConnection)sqlConnection;
            Timeout = DEFAULT_TIMEOUT;
        }

        public IAdoCommand CreateCommand()
        {
            var dbCommand = SqlConnection.CreateCommand();
            var command = new AdoCommand(dbCommand);

            return command;
        }

        public DataTable GetSchemaTable(string tableName)
        {
            const string SQL = "SELECT TOP 0 * FROM {0}"; // Get table metadata

            DataTable dataTable;

            try
            {
                var adoCommand = CreateCommand();
                adoCommand.CommandText = string.Format(SQL, tableName);

                using (var rdr = Execute(adoCommand))
                {
                    dataTable = new DataTable
                    {
                        TableName = tableName
                    };

                    dataTable.BeginLoadData();
                    dataTable.Load(rdr);
                    dataTable.EndLoadData();
                }
            }
            catch
            {
                dataTable = null;
            }

            return dataTable;
        }

        public void Execute(ICollection<IAdoCommand> adoCommands, bool useSingleTransaction = false)
        {
            if (adoCommands == null || !adoCommands.Any())
            {
                throw new ArgumentNullException("IAdoCommand cannot be null nor empty to be executed", "adoCommand");
            }

            var transactionList = new Dictionary<IDbConnection, IDbTransaction>();
            try
            {
                foreach (var command in adoCommands.Select(ConvertCommand))
                {
                    if (Timeout > command.Timeout)
                    {
                        command.Timeout = Timeout;
                    }

                    ManageTransactions(command, useSingleTransaction, transactionList);

                    var rdr = command.Execute(keepConnectionOpen: useSingleTransaction);
                    rdr.Close();
                }

                foreach (var tran in transactionList.Values)
                    tran.Commit();

            }
            catch
            {
                foreach (var tran in transactionList.Values)
                    tran.Rollback();


                throw;
            }
            finally
            {
                foreach (var conn in transactionList.Keys)
                {
                    try
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        public IDataReader Execute(IAdoCommand adoCommand)
        {
            var command = ConvertCommand(adoCommand);

            if (Timeout > command.Timeout)
            {
                command.Timeout = Timeout;
            }

            var res = command.Execute();

            return res;
        }

        public IDataReader Execute(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentNullException("Expected command text", "commandText");
            }

            var command = CreateCommand();
            command.CommandText = commandText;

            return Execute(command);
        }

        static AdoCommand ConvertCommand(IAdoCommand adoCommand)
        {
            if (adoCommand == null)
            {
                throw new ArgumentNullException("IAdoCommand cannot be null to be executed", "adoCommand");
            }

            var command = adoCommand as AdoCommand;

            if (command == null)
            {
                throw new ArgumentException("Unknown IAdoCommand", "adoCommand");
            }

            return command;
        }

        static void ManageTransactions(AdoCommand command, bool useSingleTransaction, Dictionary<IDbConnection, IDbTransaction> transactions)
        {
            if (!useSingleTransaction)
            {
                return;
            }

            var connection = command.Connection;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            IDbTransaction transaction;

            if (!transactions.ContainsKey(connection))
            {
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                transactions.Add(connection, transaction);
            }
            else
            {
                transaction = transactions[connection];
            }

            command.Transaction = transaction;
        }

        public void BulkInsert(DataTable dataTable, int batchSize = 5000, SqlBulkCopyOptions? options = null)
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    SqlConnection.Open();
                }

                #region SqlBulkCopyOptions

                // src: https://msdn.microsoft.com/pt-br/library/system.data.sqlclient.sqlbulkcopyoptions(v=vs.110).aspx
                //
                // CheckConstraints: Check constraints while data is being inserted. By default, constraints are not checked.
                // KeepNulls: Preserve null values in the destination table regardless of the settings for default values. When not specified, null values are replaced by default values where applicable.
                // TableLock: Obtain a bulk update lock for the duration of the bulk copy operation. When not specified, row locks are used.
                // UseInternalTransaction: When specified, each batch of the bulk-copy operation will occur within a transaction. If you indicate this option and also provide a SqlTransaction object to the constructor, an ArgumentException occurs.

                #endregion

                const SqlBulkCopyOptions SQL_BULK_COPY_OPTIONS = SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction;

                var sqlBulkCopy = new SqlBulkCopy(SqlConnection, options ?? SQL_BULK_COPY_OPTIONS, null)
                {
                    BatchSize = batchSize,
                    DestinationTableName = dataTable.TableName
                };

                var timeout = Convert.ToInt32(Timeout.TotalSeconds);

                sqlBulkCopy.BulkCopyTimeout = timeout > sqlBulkCopy.BulkCopyTimeout ? timeout : sqlBulkCopy.BulkCopyTimeout;

                foreach (var dataColumn in dataTable.Columns.Cast<DataColumn>().Where(c => !(c is IgnoredDataColumn)))
                {
                    sqlBulkCopy.ColumnMappings.Add(dataColumn.ColumnName, dataColumn.ColumnName);
                }

                foreach (var dataRow in dataTable.Rows.Cast<DataRow>().Where(dataRow => dataRow.RowState == DataRowState.Unchanged))
                    dataRow.SetAdded();


                using (sqlBulkCopy)
                    sqlBulkCopy.WriteToServer(dataTable);

            }
            catch
            {
                throw;
            }
            finally
            {
                if (SqlConnection.State != ConnectionState.Closed)
                {
                    SqlConnection.Close();
                }
            }
        }

        public void BatchUpdate(DataTable dataTable, string commandText, int batchSize = 5000)
        {
            SqlTransaction transaction = null;

            try
            {
                if (SqlConnection.State != ConnectionState.Open)
                {
                    SqlConnection.Open();
                }

                transaction = SqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);

                var updateCommand = new SqlCommand(commandText, SqlConnection, transaction)
                {
                    UpdatedRowSource = UpdateRowSource.None
                };

                var timeout = Convert.ToInt32(Timeout.TotalSeconds);

                updateCommand.CommandTimeout = timeout > updateCommand.CommandTimeout ? timeout : updateCommand.CommandTimeout;

                var parameters = PARAMETERS_REGEX.Matches(commandText)
                                                 .Cast<Match>()
                                                 .Select(x => new SqlParameter
                                                 {
                                                     ParameterName = x.Value,
                                                     SourceColumn = x.Value.Replace("@", string.Empty)

                                                 }).ToList();

                updateCommand.Parameters.AddRange(parameters.ToArray());

                var sqlDataAdapter = new SqlDataAdapter
                {
                    UpdateCommand = updateCommand,
                    UpdateBatchSize = batchSize
                };

                foreach (var dataRow in dataTable.Rows.Cast<DataRow>().Where(dataRow => dataRow.RowState == DataRowState.Unchanged))
                    dataRow.SetModified();


                using (updateCommand)
                using (sqlDataAdapter)
                {
                    sqlDataAdapter.Update(dataTable);
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                throw;
            }
            finally
            {
                if (SqlConnection.State != ConnectionState.Closed)
                {
                    SqlConnection.Close();
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
                    if (SqlConnection != null)
                    {
                        SqlConnection.Dispose();
                        SqlConnection = null;
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

        #region ICloneable Members

        object ICloneable.Clone() => Clone();

        public AdoExecutor Clone()
        {
            var sql = ((SqlConnection)((ICloneable)SqlConnection).Clone());

            var cloned = new AdoExecutor(sql);
            return cloned;
        }

        #endregion
    }
}