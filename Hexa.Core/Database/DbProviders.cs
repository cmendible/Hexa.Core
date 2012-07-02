using System.Runtime.Serialization;

namespace Hexa.Core.Data
{
    public enum DbProvider
    {
        [EnumMember(Value = "MySql.Data.MySQLClient")]
        MySqlProvider,

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lite")]
        [EnumMember(Value = "System.Data.SQLite")]
        SQLiteProvider,

        [EnumMember(Value = "System.Data.SqlClient")]
        MsSqlProvider,

        [EnumMember(Value = "Oracle.DataAccess.Client")]
        OracleDataProvider,

        [EnumMember(Value = "Npgsql")]
        PostgreSQLProvider,

        [EnumMember(Value = "System.Data.SqlServerCe.3.5")]
        SqlCe,

        [EnumMember(Value = "FirebirdSql.Data.FirebirdClient")]
        Firebird
    }
}
