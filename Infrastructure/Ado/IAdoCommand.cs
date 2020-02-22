using System;
using System.Collections.Generic;
using System.Data;

namespace Infrastructure.Ado
{
    /// <summary>
    /// Encapsulate ADO.Net commands
    /// </summary>
    public interface IAdoCommand
    {
        string CommandText { get; set; }
        IEnumerable<IDbDataParameter> Parameters { get; }
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Add command parameter with value
        /// </summary>
        IEnumerable<IDbDataParameter> AddParameter(IEnumerable<KeyValuePair<string, object>> parameters);

        /// <summary>
        /// Add command parameter with value
        /// </summary>
        IDbDataParameter AddParameter(string paramName, object paramValue);

        /// <summary>
        /// Try to match query parameters with IDataRecord (cursor) fields
        /// </summary>
        IAdoCommand MatchParameters(IDataRecord parameterRow);

        /// <summary>
        /// Return complete command with parameters
        /// </summary>
        string PrintFullCommandText();
    }
}