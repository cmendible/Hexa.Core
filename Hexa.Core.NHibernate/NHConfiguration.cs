//----------------------------------------------------------------------------------------------
// <copyright file="NHConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using NHibernate.Cfg;

    public class NHConfiguration
    {
        static Configuration _configuration;

        public NHConfiguration()
        {
        }

        public NHConfiguration(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Configuration Value
        {
            get
            {
                return _configuration;
            }
        }
    }
}