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

namespace Hexa.Core.Domain
{
    using System;
    using System.ComponentModel.Composition;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;

    using Data;

    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;

    using NHibernate;
    using NHibernate.Bytecode;
    using NHibernate.Cfg;
    using NHibernate.Event;
    using NHibernate.Tool.hbm2ddl;

    using EnumExtensions = System.EnumExtensions;

    using Environment = NHibernate.Cfg.Environment;

    [Export(typeof(IUnitOfWorkFactory))]
    [Export(typeof(IDatabaseManager))]
    public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory, IDatabaseManager
    {
        #region Fields

        private static Configuration builtConfiguration;
        private static string connectionString;
        private static DbProvider dbProvider;
        private static bool inMemoryDatabase;
        private static bool validationSupported = true;

        private ISessionFactory sessionFactory;

        #endregion Fields

        #region Constructors

        public NHibernateUnitOfWorkFactory(DbProvider provider, string connectionString, string cacheProvider,
            Assembly mappingsAssembly, IoCContainer container)
        {
            NHibernateUnitOfWorkFactory.dbProvider = provider;
            NHibernateUnitOfWorkFactory.connectionString = connectionString;

            FluentConfiguration cfg = null;

            switch (dbProvider)
            {
            case DbProvider.MsSqlProvider:
            {
                cfg = Fluently.Configure().Database(MsSqlConfiguration.MsSql2008
                                                    .Raw("format_sql", "true")
                                                    .ConnectionString(connectionString))
                      .ExposeConfiguration(
                          c =>
                          c.Properties.Add(Environment.SqlExceptionConverter,
                                           typeof(SqlExceptionHandler).AssemblyQualifiedName))
                      .ExposeConfiguration(c => c.Properties.Add(Environment.DefaultSchema, "dbo"));

                break;
            }
            case DbProvider.SQLiteProvider:
            {
                cfg = Fluently.Configure().Database(SQLiteConfiguration.Standard
                                                    .Raw("format_sql", "true")
                                                    .ConnectionString(connectionString));

                inMemoryDatabase = connectionString.ToUpperInvariant().Contains(":MEMORY:");

                break;
            }
            case DbProvider.SqlCe:
            {
                cfg = Fluently.Configure().Database(MsSqlCeConfiguration.Standard
                                                    .Raw("format_sql", "true")
                                                    .ConnectionString(connectionString))
                      .ExposeConfiguration(
                          c =>
                          c.Properties.Add(Environment.SqlExceptionConverter,
                                           typeof(SqlExceptionHandler).AssemblyQualifiedName));

                validationSupported = false;

                break;
            }
            case DbProvider.Firebird:
            {
                cfg = Fluently.Configure().Database(new FirebirdConfiguration()
                                                    .Raw("format_sql", "true")
                                                    .ConnectionString(connectionString));

                break;
            }
            case DbProvider.PostgreSQLProvider:
            {
                cfg = Fluently.Configure().Database(PostgreSQLConfiguration.PostgreSQL82
                                                    .Raw("format_sql", "true")
                                                    .ConnectionString(connectionString));

                validationSupported = false;

                break;
            }
            }

            Guard.IsNotNull(cfg,
                            string.Format("Db provider {0} is currently not supported.",
                                          EnumExtensions.GetEnumMemberValue(dbProvider)));

            PropertyInfo pinfo = typeof(FluentConfiguration)
                                 .GetProperty("Configuration",
                                              BindingFlags.Instance | BindingFlags.NonPublic);

            Configuration nhConfiguration = pinfo.GetValue(cfg, null) as Configuration;
            container.RegisterInstance<NHConfiguration>(new NHConfiguration(nhConfiguration));

            cfg.Mappings(m => m.FluentMappings.Conventions.AddAssembly(typeof(NHibernateUnitOfWorkFactory).Assembly))
            .Mappings(m => m.FluentMappings.Conventions.AddAssembly(mappingsAssembly))
            .Mappings(m => m.FluentMappings.AddFromAssembly(mappingsAssembly))
            .Mappings(m => m.HbmMappings.AddFromAssembly(typeof(NHibernateUnitOfWorkFactory).Assembly))
            .Mappings(m => m.HbmMappings.AddFromAssembly(mappingsAssembly))
            .ExposeConfiguration(c => c.Properties.Add(Environment.BatchSize, "100"))
            .ExposeConfiguration(c => c.Properties.Add(Environment.UseProxyValidator, "true"));

            if (!string.IsNullOrEmpty(cacheProvider))
            {
                cfg.ExposeConfiguration(c => c.Properties.Add(Environment.CacheProvider, cacheProvider))
                //"NHibernate.Cache.HashtableCacheProvider"
                .ExposeConfiguration(c => c.Properties.Add(Environment.UseSecondLevelCache, "true"))
                .ExposeConfiguration(c => c.Properties.Add(Environment.UseQueryCache, "true"));
            }

            builtConfiguration = cfg.BuildConfiguration();
            builtConfiguration.SetProperty(Environment.ProxyFactoryFactoryClass,
                                            typeof(DefaultProxyFactoryFactory).
                                            AssemblyQualifiedName);

            // Add Listeners to NHibernate pipeline....
            SetListeners(builtConfiguration);
        }

        internal NHibernateUnitOfWorkFactory()
        {
        }

        #endregion Constructors

        #region Methods

        public IUnitOfWork Create()
        {
            this.CreateSessionFactory();

            if (inMemoryDatabase)
            {
                ISession session = this.sessionFactory.OpenSession();
                new SchemaExport(builtConfiguration).Execute(false, true, false, session.Connection, Console.Out);
                return new NHibernateUnitOfWork(this.sessionFactory);
            }

            return new NHibernateUnitOfWork(this.sessionFactory);
        }

        public void CreateDatabase()
        {
            var dbManager = new DatabaseManager(dbProvider, connectionString);

            // Check if database exists.. (and create it if needed)
            if (!dbManager.DatabaseExists())
            {
                dbManager.CreateDatabase();
                new SchemaExport(builtConfiguration).Create(false, true);

                if (dbProvider == DbProvider.MsSqlProvider)
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = "RENAME_UNIQUE_KEYS";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.ExecuteNonQuery();
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

        public bool DatabaseExists()
        {
            var dbManager = new DatabaseManager(dbProvider, connectionString);
            return dbManager.DatabaseExists();
        }

        public void DeleteDatabase()
        {
            var dbManager = new DatabaseManager(dbProvider, connectionString);

            if (dbManager.DatabaseExists())
            {
                dbManager.DropDatabase();
            }
        }

        public void RegisterSessionFactory(IoCContainer container)
        {
            this.CreateSessionFactory();
            container.RegisterInstance<ISessionFactory>(this.sessionFactory);
        }

        public void ValidateDatabaseSchema()
        {
            if (!inMemoryDatabase && validationSupported)
            {
                new SchemaValidator(builtConfiguration).Validate();
            }
        }

        private void CreateSessionFactory()
        {
            if (this.sessionFactory == null)
            {
                this.sessionFactory = builtConfiguration.BuildSessionFactory();
            }
        }

        protected virtual void SetListeners(Configuration builtConfiguration)
        {
            builtConfiguration.SetListeners(ListenerType.Flush,
            new IFlushEventListener[] { new FixedDefaultFlushEventListener() });

            builtConfiguration.SetListeners(ListenerType.FlushEntity,
            new IFlushEntityEventListener[] { new AuditFlushEntityEventListener() });

            builtConfiguration.SetListeners(ListenerType.PreInsert,
                                             builtConfiguration.EventListeners.PreInsertEventListeners.Concat(
                                                 new IPreInsertEventListener[] { new ValidateEventListener(), new AuditEventListener() }).
                                             ToArray());

            builtConfiguration.SetListeners(ListenerType.PreUpdate,
                                             builtConfiguration.EventListeners.PreUpdateEventListeners.Concat(
                                                 new IPreUpdateEventListener[] { new ValidateEventListener(), new AuditEventListener() }).
                                             ToArray());
        }

        #endregion Methods
    }
}