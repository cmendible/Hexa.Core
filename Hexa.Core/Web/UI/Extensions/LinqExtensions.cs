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

namespace Hexa.Core.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Caching;

    using log4net;

    public static class LinqExtensions
    {
        #region Fields

        private static readonly ILog _Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Fields

        #region Methods

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static IList<T> LinqCache<T>(this Table<T> query)
            where T : class
        {
            string tableName = query.Context.Mapping.GetTable(typeof(T)).TableName;
            IList<T> result = HttpContext.Current.Cache[tableName] as List<T>;

            if (result == null)
            {
                try
                {
                    using (var cn = new SqlConnection(query.Context.Connection.ConnectionString))
                    {
                        cn.Open();
                        var cmd = new SqlCommand(query.Context.GetCommand(query).CommandText, cn);
                        cmd.Notification = null;
                        cmd.NotificationAutoEnlist = true;

                        _Log.DebugFormat("Attempting to enable sql cache dependency notifications for table {0}",
                                         tableName);

                        SqlCacheDependencyAdmin.EnableNotifications(query.Context.Connection.ConnectionString);

                        string[] tables =
                            SqlCacheDependencyAdmin.GetTablesEnabledForNotifications(
                                query.Context.Connection.ConnectionString);

                        if (!tables.Contains(tableName))
                            SqlCacheDependencyAdmin.EnableTableForNotifications(
                                query.Context.Connection.ConnectionString, tableName);

                        _Log.DebugFormat("Sql cache dependency notifications for table {0} is enabled.", tableName);

                        var dependency = new SqlCacheDependency(cmd);
                        cmd.ExecuteNonQuery();

                        result = query.ToList();
                        HttpContext.Current.Cache.Insert(tableName, result, dependency);

                        _Log.DebugFormat("Table {0} is cached.", tableName);
                    }
                }
                catch (Exception ex)
                {
                    result = query.Context.GetTable<T>().ToList();
                    HttpContext.Current.Cache.Insert(tableName, result);

                    string msg = string.Format(CultureInfo.InvariantCulture,
                                               "Table {0} is cached without SqlCacheDependency!!!", tableName);

                    _Log.Warn(msg, ex);
                }
            }
            return result;
        }

        #endregion Methods
    }
}