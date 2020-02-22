using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Ado
{
    class AdoCommand : IAdoCommand
    {
        static readonly Regex PARAM_RECOGNIZER = new Regex("@[a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected IDbCommand Command { get; set; }

        public string CommandText
        {
            get { return Command.CommandText; }
            set { Command.CommandText = value; }
        }

        public IDbConnection Connection => Command.Connection;

        public IDbTransaction Transaction
        {
            get { return Command.Transaction; }
            internal set { Command.Transaction = value; }
        }

        public IEnumerable<IDbDataParameter> Parameters => Command.Parameters.Cast<IDbDataParameter>();

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromSeconds(Command.CommandTimeout); }
            set { Command.CommandTimeout = Convert.ToInt32(value.TotalSeconds); }
        }

        internal AdoCommand(IDbCommand sourceCommand)
        {
            if (sourceCommand == null)
            {
                throw new ArgumentNullException("Source command cannot be null", "sourceCommand");
            }

            Command = sourceCommand;
        }

        public IEnumerable<IDbDataParameter> AddParameter(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters must not be null", "parameters");
            }

            var addedParameters = new List<IDbDataParameter>();

            foreach (var parameter in parameters)
            {
                var addedParameter = AddParameter(parameter.Key, parameter.Value);
                addedParameters.Add(addedParameter);
            }

            return addedParameters;
        }

        public IDbDataParameter AddParameter(string paramName, object paramValue)
        {
            var parameters = Command.Parameters;

            // Evitar duplicar parametros
            if (parameters.Contains(paramName))
            {
                var existingParam = parameters[paramName];
                parameters.Remove(existingParam);
            }

            var param = Command.CreateParameter();

            param.ParameterName = paramName;
            param.Value = paramValue;

            parameters.Add(param);

            return param;
        }

        internal IDataReader Execute(bool validateParameterCount = false, bool keepConnectionOpen = true)
        {
            if (validateParameterCount)
            {
                ValidateParameterCount();
            }

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            var behavior = keepConnectionOpen ? CommandBehavior.Default : CommandBehavior.CloseConnection;

            try
            {
                var rdr = Command.ExecuteReader(behavior);
                return rdr;
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (!keepConnectionOpen && Connection.State != ConnectionState.Closed)
                    {
                        Connection.Close();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public IAdoCommand MatchParameters(IDataRecord parameterRow)
        {
            var matches = Extract(PARAM_RECOGNIZER);

            if (!matches.Any() || parameterRow == null)
            {
                // Query without parameters
                return this;
            }

            foreach (var param in matches)
            {
                int paramIndex;
                try
                {
                    // Subtract At
                    paramIndex = parameterRow.GetOrdinal(param.Substring(1));
                }
                catch (ArgumentException) { continue; }
                catch (IndexOutOfRangeException) { continue; }

                var paramValue = parameterRow[paramIndex];

                AddParameter(param, paramValue);
            }

            return this;
        }

        public string PrintFullCommandText()
        {
            var fullCommandText = Command.Parameters.Cast<SqlParameter>().Aggregate(Command.CommandText, (commandText, parameter) => commandText + Environment.NewLine + (parameter.ParameterName + " = " + parameter.Value.ToString()));

            return fullCommandText;
        }

        IEnumerable<string> Extract(Regex regex)
        {
            if (regex == null)
            {
                throw new ArgumentNullException("Regex must not be null", "regex");
            }

            var matches = regex.Matches(CommandText);
            var distinct = matches.Cast<Match>().Select(x => x.Value).Distinct();

            return distinct;
        }

        void ValidateParameterCount()
        {
            var textParams = Extract(PARAM_RECOGNIZER).Count();
            var cmdParams = Parameters.Distinct().Count();

            if (textParams != cmdParams)
            {
                throw new FormatException("Too many parameters in query text");
            }
        }
    }
}