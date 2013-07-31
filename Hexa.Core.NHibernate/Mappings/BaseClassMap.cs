//----------------------------------------------------------------------------------------------
// <copyright file="BaseClassMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using FluentNHibernate.Mapping;

    using NHibernate.Cfg;
    using NHibernate.Dialect;

    public class BaseClassMap<TEntity> : ClassMap<TEntity>
    {
        public BaseClassMap()
        {
            Configuration = ServiceLocator.GetInstance<NHConfiguration>();
            Dialect = Dialect.GetDialect(Configuration.Value.Properties);
        }

        protected NHConfiguration Configuration
        {
            get;
            private set;
        }

        protected Dialect Dialect
        {
            get;
            private set;
        }
    }
}