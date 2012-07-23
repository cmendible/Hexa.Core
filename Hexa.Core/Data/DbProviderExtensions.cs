namespace Hexa.Core.Data
{
    using System.Data.Common;

    public static class DbProviderExtensions
    {
        #region Methods

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