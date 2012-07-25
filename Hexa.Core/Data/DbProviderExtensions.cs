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
    using System.Data.Common;

    public static class DbProviderExtensions
    {
        #region Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static void ExecuteNonQuery(this DbProviderFactory provider, string connectionString, string command)
        {
            // Connect & Execute cmd..
            using (DbConnection conn = provider.CreateConnection())
            {
                conn.ConnectionString = connectionString;

                try
                {
                    conn.Open();
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = command;
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static object ExecuteScalar(this DbProviderFactory provider, string connectionString, string command)
        {
            // Connect & Execute cmd..
            using (DbConnection conn = provider.CreateConnection())
            {
                conn.ConnectionString = connectionString;

                try
                {
                    conn.Open();
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = command;
                        object ret = cmd.ExecuteScalar();
                        return ret;
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        #endregion Methods
    }
}