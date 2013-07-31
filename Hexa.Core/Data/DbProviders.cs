//----------------------------------------------------------------------------------------------
// <copyright file="DbProvider.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Data
{
    using System.Runtime.Serialization;

    public enum DbProvider
    {
        [EnumMember(Value = "MySql.Data.MySQLClient")] MySqlProvider,

        [EnumMember(Value = "System.Data.SQLite")] SQLiteProvider,

        [EnumMember(Value = "System.Data.SqlClient")] MsSqlProvider,

        [EnumMember(Value = "Oracle.DataAccess.Client")] OracleDataProvider,

        [EnumMember(Value = "Npgsql")] PostgreSQLProvider,

        [EnumMember(Value = "System.Data.SqlServerCe.3.5")] SqlCe,

        [EnumMember(Value = "FirebirdSql.Data.FirebirdClient")] Firebird
    }
}