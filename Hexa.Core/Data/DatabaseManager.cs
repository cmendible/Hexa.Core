namespace Hexa.Core.Data
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using log4net;

    /// <summary>
    /// Handles Db creation, deletion, etc.
    /// </summary>
    public class DatabaseManager
    {
        #region Fields

        public const string Firebird = "FirebirdSql.Data.FirebirdClient";
        public const string MsSqlProvider = "System.Data.SqlClient";
        public const string MySqlProvider = "MySql.Data.MySQLClient";
        public const string OracleDataProvider = "Oracle.DataAccess.Client";
        public const string PostgreSQLProvider = "Npgsql";
        public const string SqlCe = "System.Data.SqlServerCe.3.5";
        public const string SQLiteProvider = "System.Data.SQLite";

        private static readonly ILog _Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DbProviderFactory _connectionProvider;
        private readonly string _connectionString;
        private readonly string _providerName;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
        /// </summary>
        /// <param name="connectionProvider">The connection provider.</param>
        /// <param name="providerName">Name of the provider.</param>
        public DatabaseManager(DbProvider provider, string connectionString)
        {
            string providerName = provider.GetEnumMemberValue();
            this._connectionProvider = DbProviderFactories.GetFactory(providerName);
            this._providerName = providerName;
            this._connectionString = connectionString;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the db provider factory.
        /// </summary>
        /// <value>The db provider factory.</value>
        public DbProviderFactory DbProviderFactory
        {
            get
            {
                return this._connectionProvider;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the database.
        /// </summary>
        public void CreateDatabase()
        {
            _CreateDatabase(this._connectionProvider, this._connectionString, this._providerName);
        }

        /// <summary>
        /// Checks whether the database instance exists.
        /// </summary>
        /// <returns></returns>
        public bool DatabaseExists()
        {
            return _DatabaseExists(this._connectionProvider, this._connectionString, this._providerName);
        }

        /// <summary>
        /// Drops the database.
        /// </summary>
        public void DropDatabase()
        {
            this._DropDatabase(this._connectionProvider, this._connectionString, this._providerName);
        }

        private static void _ClearAllPools(string providerName)
        {
            if (providerName == MsSqlProvider)
            {
                SqlConnection.ClearAllPools();
            }

            if (providerName == PostgreSQLProvider)
            {
                Type type = Type.GetType("Npgsql.NpgsqlConnection, Npgsql", true);
                MethodInfo method = type.GetMethod("ClearAllPools", BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);
            }

            if (providerName == Firebird)
            {
                Type type = Type.GetType(
                                "FirebirdSql.Data.FirebirdClient.FbConnection, FirebirdSql.Data.FirebirdClient", true);
                MethodInfo method = type.GetMethod("ClearAllPools", BindingFlags.Static | BindingFlags.Public);
                method.Invoke(null, null);
            }
        }

        private static void _CreateDatabase(DbProviderFactory provider, string connectionString, string providerName)
        {
            string dbName, dbFile;
            string connStr = _StripDbName(connectionString, providerName, out dbName, out dbFile);
            var command = new StringBuilder(); // Build SQL Command..

            if (providerName == SQLiteProvider)
            {
                // Do nothing..
                return;
            }
            if (providerName == SqlCe)
            {
                if (File.Exists(dbName))
                {
                    File.Delete(dbName);
                }

                Type type = Type.GetType("System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe");
                PropertyInfo localConnectionString = type.GetProperty("LocalConnectionString");
                MethodInfo createDatabase = type.GetMethod("CreateDatabase");

                object engine = Activator.CreateInstance(type);
                localConnectionString.SetValue(engine, string.Format("Data Source='{0}';", dbName), null);

                createDatabase.Invoke(engine, new object[0]);

                return;
            }
            if (providerName == Firebird)
            {
                if (File.Exists(dbName))
                {
                    File.Delete(dbName);
                }

                Type type = Type.GetType("FirebirdSql.Data.FirebirdClient.FbConnection, FirebirdSql.Data.FirebirdClient");
                MethodInfo createDatabase = type.GetMethod("CreateDatabase",
                                            new[]
                {
                    typeof(string), typeof(int), typeof(bool),
                    typeof(bool)
                });

                object engine = Activator.CreateInstance(type);
                createDatabase.Invoke(engine, new object[] {connectionString, 8192, true, false});

                return;
            }
            else if (providerName == OracleDataProvider)
            {
                throw new NotImplementedException();
            }
            // PostgreSQL force UTF-8 encoding..
            else if (providerName == PostgreSQLProvider)
            {
                command.AppendFormat(CultureInfo.InvariantCulture, "CREATE DATABASE \"{0}\" WITH ENCODING = 'UTF8'",
                                     dbName);
            }
            else if (providerName == MsSqlProvider)
            {
                command.AppendFormat(CultureInfo.InvariantCulture, "CREATE DATABASE [{0}] ", dbName);

                // Handle MSSQL AttachDBFile..
                if (providerName == MsSqlProvider && !string.IsNullOrEmpty(dbFile))
                {
                    string fname = Path.GetFileNameWithoutExtension(dbFile);
                    string pathname = Path.Combine(Path.GetDirectoryName(dbFile), fname);

                    command.AppendFormat(CultureInfo.InvariantCulture,
                                         "ON PRIMARY (NAME = {0}, FILENAME = '{1}.mdf', SIZE = 10MB) " +
                                         "LOG ON (NAME = {0}_log, FILENAME = '{1}.ldf', SIZE = 2MB)",
                                         fname, pathname
                                        );
                }
            }

            _Log.DebugFormat(CultureInfo.InvariantCulture, "Creating Database '{0}..", dbName);
            provider.ExecuteNonQuery(connStr, command.ToString());
            _Log.InfoFormat(CultureInfo.InvariantCulture, "Database instance '{0}' created!", dbName);
        }

        private static bool _DatabaseExists(DbProviderFactory provider, string connectionString, string providerName)
        {
            string dbName, cmdText = null;
            string connStr = _StripDbName(connectionString, providerName, out dbName);

            try
            {
                _Log.DebugFormat(CultureInfo.InvariantCulture,
                                 "Checking if database '{0}' exists, with provider: {1}, and connectionString: {2}",
                                 dbName, providerName, connStr);

                // XXX: SQLite requires a somewhat special case.. ;)
                if (providerName == SQLiteProvider)
                {
                    if (dbName.ToUpperInvariant() == ":MEMORY:")
                    {
                        return false;
                    }
                    else
                    {
                        return File.Exists(dbName);
                    }
                }

                if (providerName == SqlCe || providerName == Firebird)
                {
                    return File.Exists(dbName);
                }

                switch (providerName)
                {
                case MsSqlProvider:
                    cmdText = string.Format(CultureInfo.InvariantCulture,
                                            "select COUNT(*) from sys.sysdatabases where name=\'{0}\'", dbName);
                    break;
                case MySqlProvider:
                    cmdText = string.Format(CultureInfo.InvariantCulture,
                                            @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}'",
                                            dbName);
                    break;
                case OracleDataProvider:
                    cmdText = "SELECT 1 FROM DUAL";
                    break;
                case PostgreSQLProvider:
                    cmdText = string.Format(CultureInfo.InvariantCulture,
                                            "select count(*) from pg_catalog.pg_database where datname = '{0}'",
                                            dbName);
                    break;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture,
                                                    "Provider {0} is not supported", providerName));
                }

                object ret = provider.ExecuteScalar(connStr, cmdText);
                int count = ret == null ? 0 : Convert.ToInt32(ret, CultureInfo.InvariantCulture);

                if (count > 0)
                {
                    return true;
                }
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _Log.Error("Database connection failed", ex);
            }

            _Log.DebugFormat(CultureInfo.InvariantCulture, "Database '{0}' does not exists", dbName);
            return false;
        }

        /// <summary>
        /// Strips out the database instance name from a connectionString.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dbName">Name of the db.</param>
        /// <returns>The newly created connection string.</returns>
        private static string _StripDbName(string connectionString, string providerName, out string dbName,
            out string dbFile)
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
            string dbname = null, dbfile = null;
            object tmp;

            // SQLServer.. minimal option..
            if (builder.TryGetValue("Initial Catalog", out tmp))
            {
                dbname = tmp.ToString();
                builder.Remove("Initial Catalog");
            }
            // SQLServer default option..
            if (builder.TryGetValue("Database", out tmp))
            {
                dbname = tmp.ToString();
                builder.Remove("Database");
            }
            // SQLite! (XXX: MsSql has 'Data Source' as a means to specify Server address)
            if ((providerName == SQLiteProvider || providerName == SqlCe) && builder.TryGetValue("Data Source", out tmp))
            {
                dbname = tmp.ToString();
                builder.Remove("Data Source");
            }
            // SQLServer (auto attach alternate)
            if (builder.TryGetValue("AttachDBFileName", out tmp))
            {
                dbfile = tmp.ToString();
                // Replace |DataDirectory| in connection string.
                dbfile = dbfile.Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory") as string);
                builder.Remove("AttachDBFileName");
            }
            // Oracle SID
            if (providerName == OracleDataProvider && builder.TryGetValue("Data Source", out tmp))
            {
                string connStr = tmp.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", "");
                Match match = Regex.Match(connStr, @"SERVICE_NAME=([^\)]+)");

                if (match.Success)
                {
                    dbname = match.Groups[1].Value;
                }

                // Try EZ-Connect method..
                if (string.IsNullOrEmpty(dbname))
                {
                    match = Regex.Match(connStr, ".*/([^$]*)$");

                    if (match.Success)
                    {
                        dbname = match.Groups[1].Value;
                    }
                }
            }

            // If no database is specified at connStr, throw error..
            if (string.IsNullOrEmpty(dbname) && string.IsNullOrEmpty(dbfile))
            {
                throw new ArgumentException("ConnectionString should specify a database name or file");
            }

            // If not catalog nor database name passed, try to obtain it from db file path.
            if (string.IsNullOrEmpty(dbname))
            {
                dbname = dbfile;
            }
            //	dbname = Path.GetFileNameWithoutExtension(dbfile);

            // Save return values..
            dbName = dbname;
            dbFile = dbfile;

            return builder.ToString();
        }

        private static string _StripDbName(string connectionString, string providerName, out string dbName)
        {
            string name, file;
            string ret = _StripDbName(connectionString, providerName, out name, out file);

            dbName = !string.IsNullOrEmpty(name) ? name : file;

            return ret;
        }

        private void _DropDatabase(DbProviderFactory provider, string connectionString, string providerName)
        {
            string dbName, dbFile;
            string connStr = _StripDbName(connectionString, providerName, out dbName, out dbFile);

            // XXX: Maybe calling this.DatabaseExists prior to deletion would allow for a cleaner error.

            if (providerName == OracleDataProvider)
            {
                throw new NotImplementedException();
            }
            else if (providerName == SQLiteProvider)
            {
                if (dbName.ToUpperInvariant() != ":MEMORY:")
                {
                    File.Delete(dbName);
                }
            }
            else if (providerName == SqlCe)
            {
                File.Delete(dbName);
            }
            else if (providerName == Firebird)
            {
                _ClearAllPools(providerName);
                File.Delete(dbName);
            }
            else if (providerName == MsSqlProvider)
            {
                _ClearAllPools(providerName);

                string cmd = string.Format(CultureInfo.InvariantCulture,
                                           "USE master; ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                                           dbName);
                this._connectionProvider.ExecuteNonQuery(connStr, cmd);
                this._connectionProvider.ExecuteNonQuery(connStr,
                        string.Format(CultureInfo.InvariantCulture,
                                      "DROP DATABASE [{0}]", dbName));
            }
            else if (providerName == PostgreSQLProvider)
            {
                this._connectionProvider.ExecuteNonQuery(connStr,
                        string.Format(CultureInfo.InvariantCulture,
                                      "DROP DATABASE \"{0}\"", dbName));
            }
            else
            {
                this._connectionProvider.ExecuteNonQuery(connStr,
                        string.Format(CultureInfo.InvariantCulture,
                                      "DROP DATABASE '{0}'", dbName));
            }
        }

        #endregion Methods
    }
}