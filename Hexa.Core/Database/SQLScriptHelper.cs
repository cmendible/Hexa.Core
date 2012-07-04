#region Header

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion Header

namespace Hexa.Core.Data
{
    using System;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class to execute scripts against a Sql Server database.
    /// </summary>
    public sealed class SqlScriptHelper
    {
        #region Constructors

        private SqlScriptHelper()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Executes the specified script.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="script">The script.</param>
        public static void Execute(string connection, string script)
        {
            try
            {
                using (var sqlConn = new SqlConnection(connection))
                {
                    using (var command = new SqlCommand())
                    {
                        command.Connection = sqlConn;
                        sqlConn.Open();
                        ExecuteCommands(command, GetCommandsFromScript(script));
                        sqlConn.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Executes the commands.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="sqlCommands">The SQL commands.</param>
        private static void ExecuteCommands(SqlCommand command, string[] sqlCommands)
        {
            foreach (string cmd in sqlCommands)
            {
                if (cmd.Length > 0)
                {
                    command.CommandText = cmd;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets the commands from the specified script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        private static string[] GetCommandsFromScript(string script)
        {
            return Regex.Split(script, "GO\r\n", RegexOptions.IgnoreCase);
        }

        #endregion Methods
    }
}