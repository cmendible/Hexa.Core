//----------------------------------------------------------------------------------------------
// <copyright file="SqlScriptHelper.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
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
        private SqlScriptHelper()
        {
        }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
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
    }
}