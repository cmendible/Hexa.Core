using System.Data.Common;

namespace Hexa.Core.Database
{
    public static class DbProviderExtensions
    {
        public static void ExecuteNonQuery(this DbProviderFactory provider, string connectionString, string command)
        {
            // Connect & Execute cmd..
            using (var conn = provider.CreateConnection())
            {
                conn.ConnectionString = connectionString;

                try
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
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

        public static object ExecuteScalar(this DbProviderFactory provider, string connectionString, string command)
        {
            // Connect & Execute cmd..
            using (var conn = provider.CreateConnection())
            {
                conn.ConnectionString = connectionString;

                try
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = command;
                        var ret = cmd.ExecuteScalar();
                        return ret;
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
