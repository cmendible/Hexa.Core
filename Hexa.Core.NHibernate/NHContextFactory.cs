#region License

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

#endregion

namespace Hexa.Core.Domain
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using Data;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using NHibernate.ByteCode.Castle;
    using NHibernate.Cfg;
    using NHibernate.Event;
    using NHibernate.Tool.hbm2ddl;
    using EnumExtensions = System.EnumExtensions;
    using Environment = NHibernate.Cfg.Environment;

    public sealed class NHContextFactory : IUnitOfWorkFactory, IDatabaseManager
    {
        private readonly DbProvider _DbProvider;
        private readonly bool _InMemoryDatabase;
        private readonly Configuration _builtConfiguration;
        private readonly string _connectionString;
        private readonly bool _validationSupported = true;
        private ISessionFactory _sessionFactory;

        public NHContextFactory(DbProvider provider, string connectionString, string cacheProvider,
                                Assembly mappingsAssembly, IoCContainer container)
        {
            _DbProvider = provider;
            _connectionString = connectionString;

            FluentConfiguration cfg = null;

            switch (_DbProvider)
            {
                case DbProvider.MsSqlProvider:
                    {
                        cfg = Fluently.Configure().Database(MsSqlConfiguration.MsSql2008
                                                                .Raw("format_sql", "true")
                                                                .ConnectionString(_connectionString))
                            .ExposeConfiguration(
                                c =>
                                c.Properties.Add(Environment.SqlExceptionConverter,
                                                 typeof (SqlExceptionHandler).AssemblyQualifiedName))
                            .ExposeConfiguration(c => c.Properties.Add(Environment.DefaultSchema, "dbo"));

                        break;
                    }
                case DbProvider.SQLiteProvider:
                    {
                        cfg = Fluently.Configure().Database(SQLiteConfiguration.Standard
                                                                .Raw("format_sql", "true")
                                                                .ConnectionString(_connectionString));

                        _InMemoryDatabase = _connectionString.ToUpperInvariant().Contains(":MEMORY:");

                        break;
                    }
                case DbProvider.SqlCe:
                    {
                        cfg = Fluently.Configure().Database(MsSqlCeConfiguration.Standard
                                                                .Raw("format_sql", "true")
                                                                .ConnectionString(_connectionString))
                            .ExposeConfiguration(
                                c =>
                                c.Properties.Add(Environment.SqlExceptionConverter,
                                                 typeof (SqlExceptionHandler).AssemblyQualifiedName));

                        _validationSupported = false;

                        break;
                    }
                case DbProvider.Firebird:
                    {
                        cfg = Fluently.Configure().Database(new FirebirdConfiguration()
                                                                .Raw("format_sql", "true")
                                                                .ConnectionString(_connectionString));

                        break;
                    }
                case DbProvider.PostgreSQLProvider:
                    {
                        cfg = Fluently.Configure().Database(PostgreSQLConfiguration.PostgreSQL82
                                                                .Raw("format_sql", "true")
                                                                .ConnectionString(_connectionString));

                        _validationSupported = false;

                        break;
                    }
            }

            Guard.IsNotNull(cfg,
                            string.Format("Db provider {0} is currently not supported.",
                                          EnumExtensions.GetEnumMemberValue(_DbProvider)));

            PropertyInfo pinfo = typeof (FluentConfiguration)
                .GetProperty("Configuration",
                             BindingFlags.Instance | BindingFlags.NonPublic);

            object nhConfiguration = pinfo.GetValue(cfg, null);
            container.RegisterInstance<Configuration>(nhConfiguration);

            cfg.Mappings(m => m.FluentMappings.Conventions.AddAssembly(typeof (NHContextFactory).Assembly))
                .Mappings(m => m.FluentMappings.Conventions.AddAssembly(mappingsAssembly))
                .Mappings(m => m.FluentMappings.AddFromAssembly(mappingsAssembly))
                .Mappings(m => m.HbmMappings.AddFromAssembly(typeof (NHContextFactory).Assembly))
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

            _builtConfiguration = cfg.BuildConfiguration();
            _builtConfiguration.SetProperty(Environment.ProxyFactoryFactoryClass,
                                            typeof (ProxyFactoryFactory).
                                                AssemblyQualifiedName);

            #region Add Listeners to NHibernate pipeline....

            _builtConfiguration.SetListeners(ListenerType.FlushEntity,
                                             new IFlushEntityEventListener[] {new AuditFlushEntityEventListener()});

            _builtConfiguration.SetListeners(ListenerType.PreInsert,
                                             _builtConfiguration.EventListeners.PreInsertEventListeners.Concat(
                                                 new IPreInsertEventListener[]
                                                     {new ValidateEventListener(), new AuditEventListener()}).
                                                 ToArray());

            _builtConfiguration.SetListeners(ListenerType.PreUpdate,
                                             _builtConfiguration.EventListeners.PreUpdateEventListeners.Concat(
                                                 new IPreUpdateEventListener[]
                                                     {new ValidateEventListener(), new AuditEventListener()}).
                                                 ToArray());

            #endregion
        }

        #region IDatabaseManager Members

        public bool DatabaseExists()
        {
            var dbManager = new DatabaseManager(_DbProvider, _connectionString);
            return dbManager.DatabaseExists();
        }

        public void CreateDatabase()
        {
            var dbManager = new DatabaseManager(_DbProvider, _connectionString);

            // Check if database exists.. (and create it if needed)
            if (!dbManager.DatabaseExists())
            {
                dbManager.CreateDatabase();
                new SchemaExport(_builtConfiguration).Create(false, true);

                if (_DbProvider == DbProvider.MsSqlProvider)
                {
                    using (var conn = new SqlConnection(_connectionString))
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

        public void ValidateDatabaseSchema()
        {
            if (!_InMemoryDatabase && _validationSupported)
                new SchemaValidator(_builtConfiguration).Validate();
        }

        public void DeleteDatabase()
        {
            var dbManager = new DatabaseManager(_DbProvider, _connectionString);

            if (dbManager.DatabaseExists())
                dbManager.DropDatabase();
        }

        #endregion

        #region IUnitOfWorkFactory Members

        public IUnitOfWork Create()
        {
            _CreateSessionFactory();

            if (_InMemoryDatabase)
            {
                ISession session = _sessionFactory.OpenSession();
                new SchemaExport(_builtConfiguration).Execute(false, true, false, session.Connection, Console.Out);
                return new NHibernateUnitOfWork(_sessionFactory);
            }

            return new NHibernateUnitOfWork(_sessionFactory);
        }

        #endregion

        private void _CreateSessionFactory()
        {
            if (_sessionFactory == null)
                _sessionFactory = _builtConfiguration.BuildSessionFactory();
        }

        public void RegisterSessionFactory(IoCContainer container)
        {
            _CreateSessionFactory();
            container.RegisterInstance<ISessionFactory>(_sessionFactory);
        }
    }
}